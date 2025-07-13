using UnityEngine;
using System.Collections.Generic;

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
        public int CharactereId;

        public Vector2 Position { get; set; }
        public List<string> ConnectedNodeGuids { get; set; }
    }

    [System.Serializable]
    public class StartNodeData
    {
        public Vector2 Position { get; set; }
        public List<string> ConnectedNodeGuids { get; set; }
    }

    public StartNodeData StartNode;
    public List<DialogueLine> DialogueLines;
}