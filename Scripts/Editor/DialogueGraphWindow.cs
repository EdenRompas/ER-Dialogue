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
    private Label _fileNameLabel;

    [MenuItem("Tools/ER Dialogue")]
    public static void ShowWindow()
    {
        var window = GetWindow<DialogueGraphWindow>("ER Dialogue");
        window.Show();

        window._dialogueGraphView.ClearGraph();
        window._dialogueGraphView.CreateStartNode();
    }

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

        var leftContainer = new VisualElement();
        leftContainer.style.flexDirection = FlexDirection.Row;
        leftContainer.style.flexGrow = 1;

        var addNodeButton = new Button(() => _dialogueGraphView.CreateNode())
        {
            text = "Add Node"
        };
        leftContainer.Add(addNodeButton);

        var centerContainer = new VisualElement();
        centerContainer.style.flexDirection = FlexDirection.Row;
        centerContainer.style.justifyContent = Justify.Center;
        centerContainer.style.alignItems = Align.Center;
        centerContainer.style.flexGrow = 1;

        _fileNameLabel = new Label
        {
            text = "No File Selected",
            style =
            {
                unityFontStyleAndWeight = FontStyle.Bold,
                fontSize = 12
            }
        };
        centerContainer.Add(_fileNameLabel);

        var rightContainer = new VisualElement();
        rightContainer.style.flexDirection = FlexDirection.Row;
        rightContainer.style.justifyContent = Justify.FlexEnd;
        rightContainer.style.flexGrow = 1;

        var saveButton = new Button(() => Save())
        {
            text = "Save"
        };
        rightContainer.Add(saveButton);

        var saveNewButton = new Button(() => SaveAsNew())
        {
            text = "Save New"
        };
        rightContainer.Add(saveNewButton);

        toolbar.Add(leftContainer);
        toolbar.Add(centerContainer);
        toolbar.Add(rightContainer);

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
                    IsShowIcon = dv.IsShowIcon,
                    IconPosition = dv.IconPosition,
                    CharacterSprite = dv.CharacterSprite,
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
        Debug.Log($"Saved dialogue nodes.");
    }

    private void SaveAsNew()
    {
        string path = EditorUtility.SaveFilePanelInProject(
            "Save New Dialogue",
            "New Dialogue",
            "asset",
            "Please enter a file name to save the dialogue."
        );

        if (string.IsNullOrEmpty(path)) return;

        var newSO = CreateInstance<SO_Dialogue>();

        var oldSO = _dialogueSO;

        _dialogueSO = newSO;
        Save();

        _dialogueSO = oldSO;

        AssetDatabase.CreateAsset(newSO, path);
        AssetDatabase.SaveAssets();
        EditorUtility.SetDirty(newSO);

        Debug.Log($"New dialogue saved as: {path}");
    }

    private void Load(SO_Dialogue dialogueSO)
    {
        _fileNameLabel.text = dialogueSO.name;
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
                nodeData.IsShowIcon,
                nodeData.IconPosition,
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