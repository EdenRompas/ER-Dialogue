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
        public string Guid { get; set; }

        public string CharacterName;

        [Space]
        [TextArea(3, 5)]
        public List<string> Lines;

        [Space]
        public Sprite CharacterSprite;
        public IconPosition IconPosition;

        public bool IsShowIcon { get; set; }
        public Vector2 Position { get; set; }
        public List<string> ConnectedNodeGuids { get; set; }
    }

    [System.Serializable]
    public class StartNodeData
    {
        public Vector2 Position;
        public List<string> ConnectedNodeGuids;
    }

    [HideInInspector]
    public StartNodeData StartNode;

    [HideInInspector]
    public List<DialogueLine> DialogueLines;
}