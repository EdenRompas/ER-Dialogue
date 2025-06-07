using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class DialogueManager : MonoBehaviour
{
    public Action<string, Sprite, string, int> OnShowDialogueAndGetData;
    public Action OnShowDialogue;
    public Action OnEndDialogue;

    [SerializeField] private InputActionReference _nextDialogueInput;

    private SO_Dialogue _currentData;
    private int _currentLineIndex = 0;
    private int _currentCharacterIndex = 0;
    private bool _isDialogueActive = false;

    private void OnEnable()
    {
        _nextDialogueInput.action.performed += NextDialogue;
    }

    private void OnDisable()
    {
        _nextDialogueInput.action.performed -= NextDialogue;
    }

    private void NextDialogue(InputAction.CallbackContext context)
    {
        if (_isDialogueActive)
        {
            NextLine();
        }
    }

    public void StartDialogue(SO_Dialogue data)
    {
        _currentData = data;
        _currentLineIndex = 0;
        _currentCharacterIndex = 0;
        _isDialogueActive = true;

        ShowCurrentLine();
    }

    private void NextLine()
    {
        if (_currentData == null) return;

        var currentCharacter = _currentData.DialogueLines[_currentCharacterIndex];
        _currentLineIndex++;

        if (_currentLineIndex >= currentCharacter.Lines.Count)
        {
            _currentCharacterIndex++;
            _currentLineIndex = 0;
        }

        if (_currentCharacterIndex >= _currentData.DialogueLines.Count)
        {
            EndDialogue();
            return;
        }

        ShowCurrentLine();
    }

    private void ShowCurrentLine()
    {
        var currentCharacter = _currentData.DialogueLines[_currentCharacterIndex];
        string line = currentCharacter.Lines[_currentLineIndex];

        OnShowDialogueAndGetData?.Invoke(currentCharacter.CharacterName, currentCharacter.CharacterSprite, line, currentCharacter.CharactereId);
        OnShowDialogue?.Invoke();
    }

    private void EndDialogue()
    {
        _isDialogueActive = false;
        OnEndDialogue?.Invoke();
        _currentData = null;
    }
}
