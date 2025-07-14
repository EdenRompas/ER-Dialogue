using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(SO_Dialogue))]
public class SO_DialogueEditor : Editor
{
    private SerializedProperty _dialogueLinesProp;

    private void OnEnable()
    {
        _dialogueLinesProp = serializedObject.FindProperty("DialogueLines");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.LabelField("Dialogue Lines", EditorStyles.boldLabel);

        EditorGUI.indentLevel++;

        for (int i = 0; i < _dialogueLinesProp.arraySize; i++)
        {
            var lineProp = _dialogueLinesProp.GetArrayElementAtIndex(i);
            var characterNameProp = lineProp.FindPropertyRelative("CharacterName");
            var linesListProp = lineProp.FindPropertyRelative("Lines");
            var characterSpriteProp = lineProp.FindPropertyRelative("CharacterSprite");
            var iconPositionProp = lineProp.FindPropertyRelative("IconPosition");

            EditorGUILayout.BeginVertical("box");
            EditorGUI.BeginDisabledGroup(true);

            EditorGUILayout.PropertyField(characterNameProp);
            EditorGUILayout.PropertyField(characterSpriteProp);
            EditorGUILayout.PropertyField(iconPositionProp);
            EditorGUILayout.PropertyField(linesListProp, true);

            EditorGUI.EndDisabledGroup();
            EditorGUILayout.EndVertical();
        }

        EditorGUI.indentLevel--;

        GUILayout.Space(10);

        if (GUILayout.Button("Edit"))
        {
            var targetSO = (SO_Dialogue)target;
            DialogueGraphWindow.OpenDialogueGraphWindow(targetSO);
        }

        serializedObject.ApplyModifiedProperties();
    }

}