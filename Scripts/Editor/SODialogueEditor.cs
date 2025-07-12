using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(SO_Dialogue))]
public class SO_DialogueEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        GUILayout.Space(10);
        if (GUILayout.Button("Edit"))
        {
            DialogueNodeEditor.OpenWindow((SO_Dialogue)target);
        }
    }
}
