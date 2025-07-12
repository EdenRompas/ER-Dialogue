using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using System.Collections.Generic;

public class DialogueNodeEditor : EditorWindow
{
    private List<DialogueNode> nodes = new List<DialogueNode>();
    private List<DialogueConnection> connections = new List<DialogueConnection>();
    private DialogueNode connectingNode = null;
    private DialogueNode selectedNode = null;
    private SO_Dialogue currentSO = null;

    [MenuItem("Tools/Dialogue Node Editor")]
    public static void OpenWindow(SO_Dialogue dialogueSO)
    {
        var window = GetWindow<DialogueNodeEditor>("Dialogue Editor");
        window.LoadFromSO(dialogueSO);
    }

    private void OnGUI()
    {
        ProcessEvents(Event.current);

        GUILayout.Space(5);

        EditorGUILayout.BeginHorizontal();

        if (currentSO != null)
        {
            EditorGUILayout.BeginVertical();
            GUILayout.Label("Editing: " + currentSO.name, EditorStyles.boldLabel);
            EditorGUILayout.EndVertical();
        }
        else
        {
            EditorGUILayout.LabelField("No SO selected", EditorStyles.helpBox);
        }

        GUILayout.FlexibleSpace();

        EditorGUILayout.BeginHorizontal(GUILayout.Width(320));
        if (GUILayout.Button("Add Node", GUILayout.Height(24)))
        {
            nodes.Add(new DialogueNode(new Vector2(100, 100)));
        }

        EditorGUI.BeginDisabledGroup(currentSO == null);
        if (GUILayout.Button("Save to Current SO", GUILayout.Height(24)))
        {
            SaveToCurrentSO();
        }
        EditorGUI.EndDisabledGroup();

        if (GUILayout.Button("Save New SO", GUILayout.Height(24)))
        {
            SaveToSO();
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.EndHorizontal();

        GUILayout.Space(5);

        DrawConnections();

        BeginWindows();
        for (int i = 0; i < nodes.Count; i++)
        {
            nodes[i].rect = GUI.Window(i, nodes[i].rect, DrawNodeWindow, "Chat");
        }
        EndWindows();

        if (connectingNode != null)
        {
            Handles.DrawBezier(
                new Vector3(connectingNode.rect.xMax, connectingNode.rect.center.y),
                Event.current.mousePosition,
                new Vector3(connectingNode.rect.xMax + 50, connectingNode.rect.center.y),
                Event.current.mousePosition + Vector2.left * 50,
                Color.yellow, null, 2f
            );
            Repaint();
        }

        foreach (var node in nodes)
        {
            node.UpdateHeight();
        }
    }

    private void DrawNodeWindow(int id)
    {
        DialogueNode node = nodes[id];

        EditorGUILayout.LabelField("Name");
        node.characterName = EditorGUILayout.TextField(node.characterName);

        EditorGUILayout.Space(4);
        node.DrawReorderableList();

        EditorGUILayout.Space(4);
        node.characterSprite = (Sprite)EditorGUILayout.ObjectField("Icon", node.characterSprite, typeof(Sprite), false);

        EditorGUILayout.Space(4);
        node.characterId = EditorGUILayout.IntField("Character ID", node.characterId);

        GUILayout.Space(4);
        GUILayout.BeginHorizontal();

        if (GUILayout.Button("Delete"))
        {
            connections.RemoveAll(c => c.fromNode == node || c.toNode == node);
            nodes.Remove(node);
            GUIUtility.ExitGUI();
        }

        bool alreadyConnected = connections.Exists(c => c.fromNode == node);
        EditorGUI.BeginDisabledGroup(alreadyConnected);
        if (GUILayout.Button("Connect to")) connectingNode = node;
        EditorGUI.EndDisabledGroup();

        GUILayout.EndHorizontal();
        GUI.DragWindow();
    }

    private void DrawConnections()
    {
        for (int i = 0; i < connections.Count; i++)
        {
            var conn = connections[i];

            Vector3 start = new Vector3(conn.fromNode.rect.xMax, conn.fromNode.rect.center.y);
            Vector3 end = new Vector3(conn.toNode.rect.xMin, conn.toNode.rect.center.y);
            Vector3 startTan = start + Vector3.right * 50;
            Vector3 endTan = end + Vector3.left * 50;

            Handles.DrawBezier(start, end, startTan, endTan, Color.white, null, 3f);

            Vector3 mid = (start + end) * 0.5f;
            Rect buttonRect = new Rect(mid.x - 10, mid.y - 10, 20, 20);
            if (GUI.Button(buttonRect, "X"))
            {
                connections.RemoveAt(i);
                return;
            }
        }
    }

    private void ProcessEvents(Event e)
    {
        if (e.type == EventType.MouseDown && e.button == 0)
        {
            selectedNode = GetNodeAtPoint(e.mousePosition);
            if (selectedNode != null)
            {
                if (connectingNode != null && connectingNode != selectedNode)
                {
                    bool alreadyConnected = connections.Exists(c => c.fromNode == connectingNode);
                    if (!alreadyConnected)
                        connections.Add(new DialogueConnection(connectingNode, selectedNode));

                    connectingNode = null;
                    e.Use();
                }
                else if (connectingNode == selectedNode)
                {
                    connectingNode = null;
                    e.Use();
                }
            }
            else
            {
                connectingNode = null;
            }
        }
        else if (e.type == EventType.MouseDrag && selectedNode != null)
        {
            selectedNode.Drag(e.delta);
            e.Use();
        }
        else if (e.type == EventType.MouseUp)
        {
            selectedNode = null;
        }
    }

    DialogueNode GetNodeAtPoint(Vector2 point)
    {
        for (int i = nodes.Count - 1; i >= 0; i--)
        {
            if (nodes[i].rect.Contains(point))
                return nodes[i];
        }
        return null;
    }

    private void LoadFromSO(SO_Dialogue dialogueSO)
    {
        currentSO = dialogueSO;
        nodes.Clear();
        connections.Clear();

        if (dialogueSO.DialogueLines == null)
        {
            return;
        }

        for (int i = 0; i < dialogueSO.DialogueLines.Count; i++)
        {
            var line = dialogueSO.DialogueLines[i];
            var node = new DialogueNode(new Vector2(100 + i * 350, 100));
            node.characterName = line.CharacterName;
            node.characterSprite = line.CharacterSprite;
            node.characterId = line.CharactereId;
            node.lines = new List<string>();
            node.ReinitList();
            nodes.Add(node);
        }

        for (int i = 0; i < nodes.Count - 1; i++)
        {
            connections.Add(new DialogueConnection(nodes[i], nodes[i + 1]));
        }
    }

    private void SaveToCurrentSO()
    {
        if (currentSO == null) return;

        currentSO.DialogueLines = new List<SO_Dialogue.DialogueLine>();
        List<DialogueNode> ordered = GetOrderedNodes();

        foreach (var node in ordered)
        {
            var line = new SO_Dialogue.DialogueLine
            {
                CharacterName = node.characterName,
                Lines = new List<string>(node.lines),
                CharacterSprite = node.characterSprite,
                CharactereId = node.characterId
            };
            currentSO.DialogueLines.Add(line);
        }

        EditorUtility.SetDirty(currentSO);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log("Dialogue SO updated!");
    }



    void SaveToSO()
    {
        string path = EditorUtility.SaveFilePanelInProject("Save Dialogue SO", "NewDialogue", "asset", "Save dialogue SO");
        if (string.IsNullOrEmpty(path)) return;

        SO_Dialogue newSO = CreateInstance<SO_Dialogue>();
        newSO.DialogueLines = new List<SO_Dialogue.DialogueLine>();

        List<DialogueNode> ordered = GetOrderedNodes();
        foreach (var node in ordered)
        {
            var line = new SO_Dialogue.DialogueLine
            {
                CharacterName = node.characterName,
                Lines = new List<string>(node.lines),
                CharacterSprite = node.characterSprite,
                CharactereId = node.characterId
            };
            newSO.DialogueLines.Add(line);
        }

        AssetDatabase.CreateAsset(newSO, path);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        EditorUtility.FocusProjectWindow();
        Selection.activeObject = newSO;

        Debug.Log("SO saved to: " + path);
    }

    List<DialogueNode> GetOrderedNodes()
    {
        List<DialogueNode> ordered = new List<DialogueNode>();
        if (nodes.Count == 0) return ordered;

        DialogueNode current = nodes[0];
        ordered.Add(current);

        while (true)
        {
            DialogueConnection next = connections.Find(c => c.fromNode == current);
            if (next == null) break;

            current = next.toNode;
            if (ordered.Contains(current)) break;
            ordered.Add(current);
        }

        return ordered;
    }   
}