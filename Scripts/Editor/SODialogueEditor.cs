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

        EditorGUILayout.LabelField("Dialogue Data", EditorStyles.boldLabel);
        EditorGUILayout.HelpBox("Modify the dialogue through the graph window, and don’t forget to save your progress", MessageType.Info);

        if (GUILayout.Button("Edit"))
        {
            var targetSO = (SO_Dialogue)target;
            DialogueGraphWindow.OpenDialogueGraphWindow(targetSO);
        }

        GUILayout.Space(10);

        EditorGUI.indentLevel++;

        for (int i = 0; i < _dialogueLinesProp.arraySize; i++)
        {
            var lineProp = _dialogueLinesProp.GetArrayElementAtIndex(i);
            var characterNameProp = lineProp.FindPropertyRelative("CharacterName");
            var linesListProp = lineProp.FindPropertyRelative("Lines");
            var characterSpriteProp = lineProp.FindPropertyRelative("CharacterSprite");
            var iconPositionProp = lineProp.FindPropertyRelative("IconPosition");
            var isShowIconProp = lineProp.FindPropertyRelative("IsShowIcon");
            var positionProp = lineProp.FindPropertyRelative("Position");
            var connectedNodeGuidProp = lineProp.FindPropertyRelative("ConnectedNodeGuids");

            var guidProp = lineProp.FindPropertyRelative("Guid");
            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.PropertyField(guidProp);
            EditorGUI.EndDisabledGroup();


            EditorGUILayout.BeginVertical("box");
            EditorGUI.BeginDisabledGroup(true);

            EditorGUILayout.PropertyField(characterNameProp);
            EditorGUILayout.PropertyField(characterSpriteProp);
            EditorGUILayout.PropertyField(iconPositionProp);
            EditorGUILayout.PropertyField(linesListProp, true);

            EditorGUILayout.PropertyField(isShowIconProp);
            EditorGUILayout.PropertyField(positionProp);
            EditorGUILayout.PropertyField(connectedNodeGuidProp);

            EditorGUI.EndDisabledGroup();
            EditorGUILayout.EndVertical();
        }

        EditorGUI.indentLevel--;

        serializedObject.ApplyModifiedProperties();
    }
}