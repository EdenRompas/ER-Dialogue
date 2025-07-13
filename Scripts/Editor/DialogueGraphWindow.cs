using UnityEditor.UIElements;
using UnityEditor;
using UnityEngine.UIElements;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.Experimental.GraphView;

public class DialogueGraphWindow : EditorWindow
{
    private DialogueGraphView _dialogueGraphView;
    private SO_Dialogue _dialogueSO;

    public static void OpenDialogueGraphWindow(SO_Dialogue dialogueSO)
    {
        DialogueGraphWindow window = GetWindow<DialogueGraphWindow>("Dialogue Graph");
        window._dialogueSO = dialogueSO;
        window.Show();

        if (dialogueSO != null)
        {
            window.Load(dialogueSO);
        }
    }

    public void CreateGUI()
    {
        _dialogueGraphView = new DialogueGraphView()
        {
            name = "Dialogue Graph"
        };

        _dialogueGraphView.StretchToParentSize();
        rootVisualElement.Add(_dialogueGraphView);

        GenerateToolbar();
    }

    private void GenerateToolbar()
    {
        var toolbar = new Toolbar();

        var saveButton = new Button(() => Save())
        {
            text = "Save"
        };
        toolbar.Add(saveButton);

        var addNodeButton = new Button(() => _dialogueGraphView.CreateNode())
        {
            text = "Add Node"
        };
        toolbar.Add(addNodeButton);

        rootVisualElement.Add(toolbar);
    }

    private void Save()
    {
        if (_dialogueSO == null) return;

        _dialogueSO.DialogueLines = new List<SO_Dialogue.DialogueLine>();

        StartNode startNode = null;
        foreach (var node in _dialogueGraphView.nodes)
        {
            if (node is StartNode sn)
            {
                startNode = sn;
                break;
            }
        }

        if (startNode == null)
        {
            Debug.LogWarning("Start node not found.");
            return;
        }

        var startData = new SO_Dialogue.StartNodeData
        {
            Position = startNode.GetPosition().position,
            ConnectedNodeGuids = new List<string>()
        };

        foreach (var edge in startNode.OutputPort.connections)
        {
            if (edge.input.node is DialogueNode targetNode)
            {
                startData.ConnectedNodeGuids.Add(targetNode.Guid);
            }
        }

        _dialogueSO.StartNode = startData;

        var visited = new HashSet<DialogueNode>();
        var queue = new Queue<Node>();
        queue.Enqueue(startNode);

        while (queue.Count > 0)
        {
            Node currentNode = queue.Dequeue();

            if (currentNode is DialogueNode dv && !visited.Contains(dv))
            {
                visited.Add(dv);

                var newLine = new SO_Dialogue.DialogueLine
                {
                    Guid = dv.Guid,
                    CharacterName = dv.CharacterName,
                    Lines = new List<string>(dv.Lines),
                    CharacterSprite = dv.CharacterSprite,
                    CharactereId = dv.CharacterId,
                    Position = dv.GetPosition().position,
                    ConnectedNodeGuids = new List<string>()
                };

                foreach (var edge in dv.OutputPort.connections)
                {
                    if (edge.input.node is DialogueNode connectedNode)
                    {
                        newLine.ConnectedNodeGuids.Add(connectedNode.Guid);
                    }
                }

                _dialogueSO.DialogueLines.Add(newLine);
            }

            var outputPort = (currentNode is StartNode sn) ? sn.OutputPort :
                             (currentNode is DialogueNode dn) ? dn.OutputPort : null;

            if (outputPort == null) continue;

            foreach (var edge in outputPort.connections)
            {
                if (edge.input.node is Node nextNode && !queue.Contains(nextNode))
                {
                    queue.Enqueue(nextNode);
                }
            }
        }

        EditorUtility.SetDirty(_dialogueSO);
        AssetDatabase.SaveAssets();
        Debug.Log($"Saved {_dialogueSO.DialogueLines.Count} connected dialogue nodes.");
    }

    private void Load(SO_Dialogue dialogueSO)
    {
        _dialogueSO = dialogueSO;
        _dialogueGraphView.ClearGraph();

        Dictionary<string, DialogueNode> createdNodes = new();

        if (dialogueSO.DialogueLines == null)
        {
            _dialogueGraphView.CreateStartNode();

            return;
        }

        foreach (var nodeData in dialogueSO.DialogueLines)
        {
            var node = new DialogueNode();
            node.InitializeNodeData(
                nodeData.CharacterName,
                nodeData.CharactereId,
                nodeData.CharacterSprite,
                nodeData.Lines
            );

            node.Guid = nodeData.Guid;
            node.SetPosition(new Rect(nodeData.Position, new Vector2(200, 150)));
            _dialogueGraphView.AddElement(node);
            createdNodes.Add(nodeData.Guid, node);
        }

        foreach (var nodeData in dialogueSO.DialogueLines)
        {
            if (!createdNodes.ContainsKey(nodeData.Guid)) continue;

            var fromNode = createdNodes[nodeData.Guid];

            foreach (var targetGuid in nodeData.ConnectedNodeGuids)
            {
                if (createdNodes.TryGetValue(targetGuid, out var toNode))
                {
                    var edge = fromNode.OutputPort.ConnectTo(toNode.InputPort);
                    _dialogueGraphView.AddElement(edge);
                }
            }
        }

        if (dialogueSO.StartNode != null)
        {
            var startNode = new StartNode();
            startNode.SetPosition(new Rect(dialogueSO.StartNode.Position, new Vector2(200, 100)));
            _dialogueGraphView.AddElement(startNode);

            foreach (var targetGuid in dialogueSO.StartNode.ConnectedNodeGuids)
            {
                if (createdNodes.TryGetValue(targetGuid, out var toNode))
                {
                    var edge = startNode.OutputPort.ConnectTo(toNode.InputPort);
                    _dialogueGraphView.AddElement(edge);
                }
            }
        }
    }
}