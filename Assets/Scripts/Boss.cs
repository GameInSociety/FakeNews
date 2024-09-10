using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using JetBrains.Annotations;
using System.Linq;
using UnityEditor.ShaderKeywordFilter;

public class Boss : Interactable
{
    public static Boss Instance;

    [Header("Phrases")]

    public string phrase_OnStart = "Salut !";
    public string phrase_endOfDay = "C'est bon pour aujourd'hui";
    public string phrase_debutQuete = "Ta prochaine photo s'appelle ...";
    public string phrase_NoMorePictureNeeded = "Je n'ai plus besoin de photo aujourd'hui";
    public string phrase_PhotoCheck = "Je suis en train de regarder la photo";
    public string phrase_MissingItems = "Il manque des choses sur la photo";
    public string phrase_NothingIsGoodOnPhoto = "Rien ne va sur la photo";
    public string phrase_CorrectPhoto = "Super boulot !";
    public string phrase_QuestEnd = "Je n'ai plus rien à te dire rentre chez toi";

    public GameObject clickFeedback;
    private bool canTake = true;

    public Text uiText;

    public Photo submittedPhoto;

    int typeIndex;
    float typeTimer = 0f;
    public int typeCount = 2;
    public float typeRate = 0.1f;
    float dialog_timer = 0f;
    public float dialog_rate = 5f;
    public bool dialog_speaking = false;

    public LookHead lookHead;

    public Transform photo_anchor;
    public CanvasGroup dialog_group;

    private void Awake() {
        Instance = this;
    }

    private void Start() {
        lookHead.SetTarget(FirstPersonController.instance.playerCamera.transform);
        dialog_group.alpha = 0f;
        camera_initpos = Camera.main.transform.localPosition;
        Invoke("StartDelay", 3f);
    }

    void StartDelay() {
        StartDialog("Hey. Toi là bas. Viens me voir pour te mettre au turbin. ");
    }
    bool finished = false;
    private void Update() {
        if (dialog_speaking) {
            UpdateDialog();
        }
    }
    bool typing = false;
    void UpdateDialog() {
        // typing
        if (finished) {
            clickFeedback.SetActive(true);
            if (Input.GetMouseButtonDown(0))
                NextDialog();
        } else {
            typeTimer += Time.deltaTime;
            if (Input.GetMouseButtonDown(0)) {
                finished = true;
                typeIndex = dialogs[dialogIndex].Length;
                uiText.text = dialogs[dialogIndex];
                return;
            }
            if (typeTimer >= typeRate) {
                typeIndex += typeCount;
                if (typeIndex >= dialogs[dialogIndex].Length) {
                    finished = true;
                    typeIndex = dialogs[dialogIndex].Length;
                    uiText.text = dialogs[dialogIndex];
                } else {
                    uiText.text = dialogs[dialogIndex].Remove(typeIndex);
                }
                typeTimer = 0f;
            }
            clickFeedback.SetActive(false);
        }

        dialog_timer += Time.deltaTime;
        
    }

    // interaction
    public override void PickUp() {

        if (QuestManager.Instance.finishedAllQuests) {
            StartDialog(phrase_endOfDay);
            return;
        }

        var quest = QuestManager.Instance.currentQuest;
        if ( quest.currentState == Quest.State.None) {
            StartDialog($"{quest.dialogue_start}");
            QuestManager.Instance.CurrentQuest_Start();
        } else {
            StartDialog($"{quest.clue}");
        }
        dialog_timer = 0f;

    }


    /// <summary>
    /// PHOTO
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log($"hit : {other.name}");
        var photo = other.GetComponent<Photo>();
        if (photo != null && canTake) {
            canTake = false;
            CheckPhoto_Start(photo);
        }
    }

    // boss takes the photo
    void CheckPhoto_Start(Photo photo) {

        submittedPhoto = photo;

        if (Interacter.Instance.holdingItem) {
            Interacter.Instance.holdingItem = false;
        }

        if (QuestManager.Instance.finishedAllQuests) {
            StartDialog(phrase_NoMorePictureNeeded);
            RejectPhoto();
            return;
        }
        able = false;

        lookHead.SetTarget(photo.transform);
        FeedbackScreen.Instance.SetPhoto(photo);

        photo.able = false;
        photo.GetComponent<Rigidbody>().useGravity = false;
        photo.GetComponent<Rigidbody>().isKinematic = true;
        photo.transform.DORotateQuaternion(photo_anchor.rotation, 0.5f);
        photo.transform.DOMove(photo_anchor.position, 0.5f);

        submittedPhoto = photo;

        if ( QuestManager.Instance.currentQuest.currentState == Quest.State.None) {
            Invoke("FreePhoto_Delay", 3f);
        } else {
            Invoke("CheckPhoto_GetInfo", 3f);
        }
    }

    void FreePhoto_Delay() {
        FeedbackScreen.Instance.SetFree();
        Invoke("FreePhoto_Delay2", 3f);

    }

    void FreePhoto_Delay2() {
        submittedPhoto.transform.DOScale(0f, 0.5f).SetEase(Ease.InBounce);
        Destroy(submittedPhoto.gameObject, 1f);
        submittedPhoto = null;
        CheckPhoto_Finish();
    }

    // 
    void CheckPhoto_GetInfo() {

        var quest = QuestManager.Instance.currentQuest;
        int targetCount = quest.item_ids.Count;
        int count= 0;
        foreach (var item in submittedPhoto.items) {
            if ( quest.item_ids.Find(x=> x == item.id) != null) {
                ++count;
            }
        }

        
        if ( count == targetCount) {
            ConfirmPhoto();
        } else {
            RejectPhoto();
            if (count > 0) {
                StartDialog(phrase_MissingItems);
            } else {
                StartDialog(phrase_NothingIsGoodOnPhoto);
            }
        }
    }


    void RejectPhoto() {
            FeedbackScreen.Instance.SetBad();
        Invoke($"RejectPhoto_Delay", 2f);
    }
    void RejectPhoto_Delay() {
        submittedPhoto.able = true;
        submittedPhoto.GetComponent<Rigidbody>().useGravity = true;
        submittedPhoto.GetComponent<Rigidbody>().isKinematic = false;
        submittedPhoto.GetComponent<Rigidbody>().AddForce(transform.forward * 300f);
        submittedPhoto = null;

        Invoke("CheckPhoto_Finish", 1f);
    }


    #region confirm
    void ConfirmPhoto() {
        FeedbackScreen.Instance.SetGood();
        StartDialog(phrase_CorrectPhoto);
        submittedPhoto.transform.DOScale(0f, 0.5f).SetEase(Ease.InBounce);
        Destroy(submittedPhoto.gameObject, 1f);
        submittedPhoto = null;

        Invoke($"ConfirmPhotoDelay", 2f);
    }

    void ConfirmPhotoDelay() {
        StartDialog(QuestManager.Instance.currentQuest.dialogue_end);
        QuestManager.Instance.CurrentQuest_Finish();
        CheckPhoto_Finish();
    }
    #endregion

    public void CheckPhoto_Finish() {
        lookHead.SetTarget(FirstPersonController.instance.playerCamera.transform);
        canTake = true;
        able = true;
    }

    /// <summary>
    /// dialgoues
    /// </summary>
    /// <param name="str"></param>
    public List<string> dialogs = new List<string>();
    public Transform cameraAnchor;
    public int dialogIndex;
    public void StartDialog(string str) {
        if (dialog_speaking) {
        Debug.Log($"Double Dialogu: {str}");
        }

        Debug.Log($"Starting Dialoge : {str}");
        able = false;
        dialog_group.transform.localScale = Vector3.zero;
        dialog_group.DOFade(1f, 0.15f);
        dialogs = str.Split(". ").ToList();
        if ( dialogs.Count > 1)
            dialogs.RemoveAt(dialogs.Count - 1);

        Menu.Instance.Pause();
        dialogIndex = 0;
        Camera.main.transform.DOMove(cameraAnchor.position, 1f);
        Camera.main.transform.DORotateQuaternion(cameraAnchor.rotation, 1f);
        CancelInvoke("SpeakCurrentDialog");
        CancelInvoke("EndDialogDelay");
        Invoke("SpeakCurrentDialog", 1f);
    }

    public void SpeakCurrentDialog() {
        dialog_speaking = true;
        finished = false;
        typeIndex = 0;
        uiText.text = "";
        dialog_timer = 0f;
        typeTimer = 0f;
        clickFeedback.SetActive(false);
        dialog_group.transform.DOScale(1.1f, 0.2f).SetEase(Ease.OutBounce);
        dialog_group.transform.DOScale(1f, 0.2f).SetEase(Ease.Linear).SetDelay(0.2f);
    }

    public void NextDialog() {
        ++dialogIndex;
        if (dialogIndex == dialogs.Count) {
            EndDialog();
            return;
        }
        SpeakCurrentDialog();
    }

    public Vector3 camera_initpos;
    public void EndDialog() {
        Camera.main.transform.DOLocalMove(camera_initpos, 1f);
        Camera.main.transform.DOLocalRotateQuaternion(Quaternion.identity, 1f);
        dialog_group.transform.DOScale(0f, 0.5f).SetEase(Ease.InBounce);
        dialog_group.DOFade(0f, 0.5f);
        uiText.text = "";

        Invoke("EndDialogDelay", 1f);
    }

    void EndDialogDelay() {
        dialog_speaking = false;
        Menu.Instance.Resume();
    }

}
