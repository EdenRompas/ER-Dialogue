using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

public class UI_Dialogue : MonoBehaviour
{
    [SerializeField] private DialogueManager _dialogueManager;
    [SerializeField] private UI_CanvasController _canvasControllerUI;

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
        _canvasControllerUI.ActivatedPanel("dialogue");
    }

    public void DeactivedDialogue()
    {
        _canvasControllerUI.DeactivatedPanel("dialogue");
    }

    public void SetDialogue(string characterName, Sprite characterSprite, string line, IconPosition iconPosition, bool isShowIcon)
    {
        _characterNameText.text = characterName;

        foreach (var item in _charactersImage)
        {
            item.gameObject.SetActive(false);
        }

        int imageId = 0;

        switch(iconPosition)
        {
            case IconPosition.Left:

                imageId = 0;

                break;

            case IconPosition.Right:

                imageId = 1;

                break;
        }

        if (characterSprite != null && isShowIcon)
        {
            _charactersImage[imageId].gameObject.SetActive(true);
            _charactersImage[imageId].sprite = characterSprite;
        }
        else
        {
            _charactersImage[imageId].gameObject.SetActive(false);
        }

        _dialogueText.text = line;
    }
}