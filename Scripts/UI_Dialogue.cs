using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

public class UI_Dialogue : MonoBehaviour
{
    [SerializeField] private DialogueManager _dialogueManager;

    [SerializeField] private GameObject _dialoguePanel;
    [SerializeField] private TMP_Text _dialogueText;
    [SerializeField] private TMP_Text _characterNameText;
    [SerializeField] private List<Image> _charactersImage;

    private void Awake()
    {
        _dialogueManager.OnShowDialogueAndGetData += SetDialogue;
        _dialogueManager.OnShowDialogue += ActivatedDialogue;
        _dialogueManager.OnEndDialogue += DeactiveDialogue;
    }

    public void ActivatedDialogue()
    {
        _dialoguePanel.SetActive(true);
    }

    public void DeactiveDialogue()
    {
        _dialoguePanel.SetActive(false);
    }

    public void SetDialogue(string characterName, Sprite characterSprite, string line, int characterId)
    {
        _characterNameText.text = characterName;

        foreach (var item in _charactersImage)
        {
            item.gameObject.SetActive(false);
        }

        _charactersImage[characterId].gameObject.SetActive(true);
        _charactersImage[characterId].sprite = characterSprite;

        _dialogueText.text = line;
    }
}