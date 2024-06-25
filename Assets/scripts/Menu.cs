using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
    public static Menu Instance;

    public RectTransform title_RectTransform;
    public RectTransform help_RectTransform;

    public CanvasGroup title_CanvasGroup;
    public CanvasGroup help_CanvasGroup;

    public float bounce_amount = 1.1f;
    public float bounce_dur = .4f;

    public Text title_Text;
    public Text help_Text;


    private void Awake()
    {
        Instance = this;
    }

    public void DisplayHelp(string msg) {
        help_Text.text = msg;
        Bounce(help_RectTransform);
    }

    public void DisplayQuestName(string msg) {
        title_Text.text = msg;
        Bounce(title_RectTransform);
    }

    public void Bounce (Transform t) {
        t.DOScale(bounce_amount, bounce_dur).SetEase(Ease.OutBounce);
        t.DOScale(1f, bounce_dur).SetDelay(bounce_dur);
    }
}
