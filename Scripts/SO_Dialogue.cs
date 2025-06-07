using UnityEngine;
using System.Collections.Generic;
using EREditor.Inspector;

[CreateAssetMenu(fileName = "NewDialogue", menuName = "Dialogue/Dialogue Data")]
public class SO_Dialogue : ScriptableObject
{
    [System.Serializable]
    public class DialogueLine
    {
        [Title("Name")]
        public string CharacterName;

        [Space]
        [Title("Text Conversation")]
        [TextArea(3, 5)]
        public List<string> Lines;

        [Space]
        [Title("Character To Display")]
        [InfoBox("If don't want to display a character, just leave it empty")]
        public Sprite CharacterSprite;

        [InfoBox("If there is no character image you want to display, just ignore this")]
        public int CharactereId;
    }

    public List<DialogueLine> DialogueLines;
}