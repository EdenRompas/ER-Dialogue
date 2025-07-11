using UnityEngine;
using System.Collections.Generic;
using System;
using DG.Tweening;

[Serializable]
public class PanelAnimationSettings
{
    public enum AnimationType { None, SlideFromBottom, SlideFromTop, PopUp }
    public AnimationType animationType;
    public float duration = 0.5f;
    public Vector3 startOffset;
    public Ease easeType = Ease.OutBack;
}

[Serializable]
public class Panel
{
    public GameObject Object;
    public GameObject[] DeactiveObjects;
    public CanvasGroup[] BackgroundCanvasGroups;
    public string Id;
    public PanelAnimationSettings AnimationSettings;

    public Transform OriginalPosition { get; set; }
}

public class UI_CanvasController : MonoBehaviour
{
    [SerializeField] private List<Panel> _panels = new List<Panel>();

    private bool _isDelayAnimate;

    private void Awake()
    {
        foreach (var item in _panels)
        {
            item.OriginalPosition = item.Object.transform;
        }
    }

    public void ActivatedPanel(string panelId)
    {
        if (!_isDelayAnimate)
        {
            foreach (var item in _panels)
            {
                if (item.Id == panelId)
                {
                    foreach (var item1 in item.DeactiveObjects)
                    {
                        item1.SetActive(false);
                    }

                    foreach (var item1 in item.BackgroundCanvasGroups)
                    {
                        item1.gameObject.SetActive(true);
                        item1.DOFade(1f, 0.2f).SetEase(Ease.OutQuad);
                        item1.interactable = true;
                        item1.blocksRaycasts = true;
                    }

                    _isDelayAnimate = true;
                    AnimatePanel(item.Object, item.OriginalPosition.localPosition, item.AnimationSettings, true, ReadyToAnimate);
                }
            }
        }
    }

    public void DeactivatedPanel(string panelId)
    {
        if (!_isDelayAnimate)
        {
            foreach (var item in _panels)
            {
                if (item.Id == panelId)
                {
                    foreach (var item1 in item.DeactiveObjects)
                    {
                        item1.SetActive(true);
                    }

                    foreach (var item1 in item.BackgroundCanvasGroups)
                    {
                        item1.DOFade(0f, 0.2f).SetEase(Ease.InQuad)
                        .OnComplete(() =>
                        {
                            item1.gameObject.SetActive(false);
                            item1.interactable = false;
                            item1.blocksRaycasts = false;
                        });
                    }

                    _isDelayAnimate = true;
                    AnimatePanel(item.Object, item.OriginalPosition.localPosition, item.AnimationSettings, false, ReadyToAnimate);
                }
            }
        }
    }

    public void ActivatedAllPanel()
    {
        foreach (var item in _panels)
        {
            item.Object.SetActive(true);
        }
    }

    public void DeactivatedAllPanel()
    {
        foreach (var item in _panels)
        {
            item.Object.SetActive(false);
        }
    }

    private void AnimatePanel(GameObject panel, Vector3 originalPosition, PanelAnimationSettings settings, bool isOpening, Action onComplete = null)
    {
        if (panel == null) return;

        panel.SetActive(true);
        Vector3 startPosition = originalPosition + settings.startOffset;

        switch (settings.animationType)
        {
            case PanelAnimationSettings.AnimationType.SlideFromBottom:
                startPosition = new Vector3(originalPosition.x, originalPosition.y - settings.startOffset.y, originalPosition.z);
                break;
            case PanelAnimationSettings.AnimationType.SlideFromTop:
                startPosition = new Vector3(originalPosition.x, originalPosition.y + settings.startOffset.y, originalPosition.z);
                break;
            case PanelAnimationSettings.AnimationType.PopUp:
                if (isOpening)
                {
                    panel.transform.localScale = Vector3.zero;
                }
                else
                {
                    panel.transform.localScale = Vector3.one;
                }
                break;
        }

        if (isOpening)
        {
            panel.transform.localPosition = startPosition;
            if (settings.animationType == PanelAnimationSettings.AnimationType.PopUp)
            {
                panel.transform.DOScale(Vector3.one, settings.duration).SetEase(settings.easeType).OnComplete(() => onComplete?.Invoke());
            }
            else
            {
                panel.transform.DOLocalMove(originalPosition, settings.duration).SetEase(settings.easeType).OnComplete(() => onComplete?.Invoke());
            }
        }
        else
        {
            if (settings.animationType == PanelAnimationSettings.AnimationType.PopUp)
            {
                panel.transform.DOScale(Vector3.zero, settings.duration).SetEase(Ease.Linear).OnComplete(() =>
                {
                    panel.SetActive(false);
                    onComplete?.Invoke();
                });
            }
            else
            {
                panel.transform.DOLocalMove(startPosition, settings.duration).SetEase(settings.easeType).OnComplete(() =>
                {
                    panel.SetActive(false);
                    onComplete?.Invoke();
                    panel.transform.localPosition = originalPosition;
                });
            }
        }
    }

    private void ReadyToAnimate()
    {
        _isDelayAnimate = false;
    }
}