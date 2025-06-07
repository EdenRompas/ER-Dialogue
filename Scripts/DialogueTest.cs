using UnityEngine;
using UnityEngine.InputSystem;

public class DialogueTest : MonoBehaviour
{
    [SerializeField] private DialogueManager _dialogueManager;
    [SerializeField] private SO_Dialogue _dialogueSO;
    [SerializeField] private InputActionReference _startDialogueInput;

    private void OnEnable()
    {
        _startDialogueInput.action.performed += StartDialogue;
    }

    private void OnDisable()
    {
        _startDialogueInput.action.performed -= StartDialogue;
    }

    private void StartDialogue(InputAction.CallbackContext context)
    {
        _dialogueManager.StartDialogue(_dialogueSO);
    }
}
