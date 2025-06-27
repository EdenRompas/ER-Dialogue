using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "Dialogue", menuName = "Dialogue/New Dialogue")]
public class SO_Dialogue : ScriptableObject
{
    [System.Serializable]
    public class DialogueLine
    {
        public string CharacterName;

        [Space]
        [TextArea(3, 5)]
        public List<string> Lines;

        [Space]
        public Sprite CharacterSprite;
        public int CharactereId;
    }

    public List<DialogueLine> DialogueLines;
}