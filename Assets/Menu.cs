using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class Menu : MonoBehaviour
{
    public GameObject title_Go;
    public GameObject help_Go;

    public RectTransform title_RectTransform;
    public RectTransform help_RectTransform;

    public float bounce_amount = 1.1f;
    public float bounce_dur = .4f;

    public Text title_Text;
    public Text help_Text;

    public Transform title_Anchor;
    public Transform help_Anchor;

    public string[] titles;
    public string[] helps;

    public int lvlIndex;

    public float displayDuration = 4f;

    private void Start()
    {
        StartCoroutine(NewLevel());
    }

    IEnumerator NewLevel() {
        title_Text.text = titles[lvlIndex];
        help_Text.text = helps[lvlIndex];

        title_Go.SetActive(true);
        help_Go.SetActive(false);

        title_RectTransform.anchoredPosition = Vector2.zero;

        yield return new WaitForSeconds(.5f);

        Bounce(title_RectTransform.transform);

        yield return new WaitForSeconds(2f);

        title_RectTransform.DOMove(title_Anchor.position, 0.5f);

        yield return new WaitForSeconds(2f);

        help_Go.SetActive(true);
        help_RectTransform.anchoredPosition = Vector2.zero;

        yield return new WaitForSeconds(.5f);

        help_RectTransform.DOMove(help_Anchor.position, 0.5f);

        Bounce(help_RectTransform.transform);


    }

    void ShowTitleDelay() {
        title_RectTransform.DOMove(title_Anchor.position, 0.5f);
    }

    public void Bounce (Transform t) {
        t.DOScale(bounce_amount, bounce_dur).SetEase(Ease.OutBounce);
        t.DOScale(1f, bounce_dur).SetDelay(bounce_dur);
    }
}
