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

    public CanvasGroup pause_group;
    public CanvasGroup menu_group;
    public float decal = 1f;

    private void Awake()
    {
        Instance = this;
    }

    private void Start() {
        pause_group.alpha = 0f;
    }

    public void Pause() {

        CameraBehavior.Instance._animator.gameObject.SetActive(false);
        CameraBehavior.Instance.Camera_Exit();
        FirstPersonController.instance.TurnToTarget(Boss.Instance.transform.position + Vector3.up * decal);
        menu_group.DOKill();
        pause_group.DOKill();
        menu_group.DOFade(0f, 1f);
        pause_group.DOFade(1f, 1f);
    }

    public void Resume() {
        menu_group.DOKill();
        pause_group.DOKill();
        menu_group.DOFade(1f, 1f);
        pause_group.DOFade(0f, 1f);
        CancelInvoke("ResumeDelay");
        Invoke("ResumeDelay", 1f);
    }
    void ResumeDelay() {
        CameraBehavior.Instance._animator.gameObject.SetActive(true);
        FirstPersonController.instance.Resume();
        Boss.Instance.able = true;
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
