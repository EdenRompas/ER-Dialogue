using UnityEngine;
using System.Collections.Generic;
using UnityEditorInternal;
using UnityEditor;

public class DialogueNode
{
    public Rect rect;
    public string characterName = "Character";
    public List<string> lines = new List<string>();
    public Sprite characterSprite;
    public int characterId;

    private ReorderableList reorderableList;

    public DialogueNode(Vector2 position)
    {
        rect = new Rect(position.x, position.y, 260, 0);

        reorderableList = new ReorderableList(lines, typeof(string), true, true, true, true);
        reorderableList.drawHeaderCallback = (Rect rect) =>
        {
            EditorGUI.LabelField(rect, "Lines");
        };
        reorderableList.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
        {
            lines[index] = EditorGUI.TextField(new Rect(rect.x, rect.y + 2, rect.width, EditorGUIUtility.singleLineHeight), lines[index]);
        };
    }

    public void DrawReorderableList()
    {
        reorderableList.DoLayoutList();
    }

    public void Drag(Vector2 delta)
    {
        rect.position += delta;
    }

    public void UpdateHeight()
    {
        float listHeight = reorderableList != null ? reorderableList.GetHeight() : 0f;
        float fixedHeight = 160;
        rect.height = Mathf.Max(0, fixedHeight + listHeight + 30);
    }
}