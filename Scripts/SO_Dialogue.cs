using UnityEngine;
using System.Collections.Generic;

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
        public List<string> Lines;

        [Space]
        public Sprite CharacterSprite;
        public int CharactereId;

        public Vector2 Position;
        public List<string> ConnectedNodeGuids;
    }

    [System.Serializable]
    public class StartNodeData
    {
        public Vector2 Position;
        public List<string> ConnectedNodeGuids;
    }

    public StartNodeData StartNode;
    public List<DialogueLine> DialogueLines;
}