using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

public class UI_Dialogue : MonoBehaviour
{
    [SerializeField] private DialogueManager _dialogueManager;
    [SerializeField] private UI_CanvasController _canvasControllerUI;

    [SerializeField] private GameObject _dialoguePanel;
    [SerializeField] private TMP_Text _dialogueText;
    [SerializeField] private TMP_Text _characterNameText;
    [SerializeField] private List<Image> _charactersImage;

    private void Awake()
    {
        _dialogueManager.OnShowDialogueAndGetData += SetDialogue;
        _dialogueManager.OnShowDialogue += ActivatedDialogue;
        _dialogueManager.OnEndDialogue += DeactivedDialogue;
    }

    public void ActivatedDialogue()
    {
        _dialoguePanel.SetActive(true);
        _canvasControllerUI.DeactivatedAllPanel();
    }

    public void DeactivedDialogue()
    {
        _dialoguePanel.SetActive(false);
        _canvasControllerUI.ActivatedAllPanel();
    }

    public void SetDialogue(string characterName, Sprite characterSprite, string line, int characterId)
    {
        _characterNameText.text = characterName;

        foreach (var item in _charactersImage)
        {
            item.gameObject.SetActive(false);
        }

        if (characterSprite != null)
        {
            _charactersImage[characterId].gameObject.SetActive(true);
            _charactersImage[characterId].sprite = characterSprite;
        }
        else
        {
            _charactersImage[characterId].gameObject.SetActive(false);
        }

        _dialogueText.text = line;
    }
}