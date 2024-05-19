using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class Menu : MonoBehaviour
{
    public static Menu Instance;

    public GameObject[] vortexes;

    bool transition = false;

    public GameObject title_Go;
    public GameObject help_Go;

    public RectTransform title_RectTransform;
    public RectTransform help_RectTransform;

    public CanvasGroup title_CanvasGroup;
    public CanvasGroup help_CanvasGroup;

    public float bounce_amount = 1.1f;
    public float bounce_dur = .4f;

    public GameObject transition_go;

    public Text title_Text;
    public Text help_Text;

    public Transform title_Anchor;
    public Transform help_Anchor;

    public string[] titles;
    public string[] helps;

    public int lvlIndex;

    public float displayDuration = 4f;


    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        title_CanvasGroup.alpha = 0f;
        help_CanvasGroup.alpha = 0f;
        
        foreach (var vortex in vortexes) {
            vortex.SetActive(false);
        }
        StartTransition();
    }

    IEnumerator TransitionCoroutine() {

        transition_go.SetActive(false);

        help_CanvasGroup.DOFade(0f, 0.2f);
        title_CanvasGroup.DOFade(0f, 0.2f);

        yield return new WaitForSeconds(2f);

        help_Go.SetActive(true);
        help_Text.text = $"Go to the computer";
        help_CanvasGroup.DOFade(1f, 0.2f);
        help_RectTransform.anchoredPosition = Vector2.zero;

        yield return new WaitForSeconds(2f);

        Bounce(help_RectTransform.transform);
        help_RectTransform.DOMove(help_Anchor.position, 0.5f);

        transition_go.SetActive(true);

    }

    public void StartTransition() {
        StartCoroutine(TransitionCoroutine());
    }

    public void NextLevel() {

        if (lvlIndex+1 == titles.Length) {
            Debug.Log($"fin du jeu");
            return;
        }

        ++lvlIndex;
        StartTransition();
    }

    public void EndLevel() {
        var fadedur = 0.5f;
        title_CanvasGroup.DOFade(0f, fadedur);
        help_CanvasGroup.DOFade(0f, fadedur);
    }

    public void StartLevel() {
        StartCoroutine(StartLevelCoroutine());
    }

    IEnumerator StartLevelCoroutine() {

        help_CanvasGroup.DOFade(0f, 0.5f);
        title_Text.text = titles[lvlIndex];
        help_Text.text = helps[lvlIndex];

        title_Go.SetActive(true);
        title_CanvasGroup.DOFade(1f, 0.5f);
        title_RectTransform.anchoredPosition = Vector2.zero;

        yield return new WaitForSeconds(.5f);

        title_CanvasGroup.DOFade(1f, 0.2f);
        Bounce(title_RectTransform.transform);

        yield return new WaitForSeconds(2f);

        title_RectTransform.DOMove(title_Anchor.position, 0.5f);

        yield return new WaitForSeconds(2f);

        help_CanvasGroup.DOFade(1f, 0.2f);
        help_RectTransform.anchoredPosition = Vector2.zero;

        yield return new WaitForSeconds(2f);

        Bounce(help_RectTransform.transform);
        help_RectTransform.DOMove(help_Anchor.position, 0.5f);

        vortexes[lvlIndex].SetActive(true);
    }

    public void Bounce (Transform t) {
        t.DOScale(bounce_amount, bounce_dur).SetEase(Ease.OutBounce);
        t.DOScale(1f, bounce_dur).SetDelay(bounce_dur);
    }
}
