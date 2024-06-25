using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using JetBrains.Annotations;

public class Boss : Interactable
{
    public static Boss Instance;

    [Header("Phrases")]

    [SerializeField]
    private string phrase_endOfDay = "C'est bon pour aujourd'hui";

    public string phrase_debutQuete = "Ta prochaine photo s'appelle ...";
    public string phrase_NoMorePictureNeeded = "Je n'ai plus besoin de photo aujourd'hui";
    public string phrase_PhotoCheck = "Je suis en train de regarder la photo";
    public string phrase_MissingItems = "Il manque des choses sur la photo";
    public string phrase_NothingIsGoodOnPhoto = "Rien ne va sur la photo";
    public string phrase_CorrectPhoto = "Super boulot !";
    public string phrase_QuestEnd = "Je n'ai plus rien à te dire rentre chez toi";

    private bool canTake = true;

    public Text uiText;

    public Photo submittedPhoto;

    float dialog_timer = 0f;
    public float dialog_rate = 5f;
    public bool dialog_speaking = false;

    public LookHead lookHead;

    public Transform photo_anchor;

    private void Awake() {
        Instance = this;
    }

    private void Start() {
        lookHead.SetTarget(FirstPersonController.instance.playerCamera.transform);
    }


    private void Update() {
        if (dialog_speaking) {
            dialog_timer += Time.deltaTime;
            if (dialog_timer >=  dialog_rate) {
                EndDialog();
            }
        }
    }

    // interaction
    public override void PickUp() {

        if (QuestManager.Instance.finishedAllQuests) {
            StartDialog(phrase_endOfDay);
            return;
        }

        var quest = QuestManager.Instance.currentQuest;
        if ( quest.currentState == Quest.State.None) {
            StartDialog($"{phrase_debutQuete} : {quest.name}");
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

        StartDialog(phrase_PhotoCheck);

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
        StartDialog(phrase_QuestEnd);
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
    public void StartDialog(string str) {
        uiText.text = str;
        dialog_timer = 0f;
        dialog_speaking = true;
    }

    public void EndDialog() {
        uiText.text = "";
        dialog_speaking = false;
    }

}
