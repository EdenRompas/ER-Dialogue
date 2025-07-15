using UnityEngine;
using System.Collections.Generic;

public enum IconPosition
{
    Left,
    Right
}

[CreateAssetMenu(fileName = "Dialogue", menuName = "Dialogue/New Dialogue")]
public class SO_Dialogue : ScriptableObject
{
    [System.Serializable]
    public class DialogueLine
    {
        public string Guid;

        public string CharacterName;

        [Space]
        [TextArea(3, 5)]
        public List<string> Lines = new List<string>();

        [Space]
        public Sprite CharacterSprite;
        public IconPosition IconPosition;

        public bool IsShowIcon;
        public Vector2 Position;
        public List<string> ConnectedNodeGuids;
    }

    [System.Serializable]
    public class StartNodeData
    {
        public Vector2 Position;
        public List<string> ConnectedNodeGuids = new List<string>();
    }

    [HideInInspector]
    public StartNodeData StartNode;

    [HideInInspector]
    public List<DialogueLine> DialogueLines = new List<DialogueLine>();
}