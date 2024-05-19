using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class Vortex : MonoBehaviour
{
    public Image image;
    private bool canTake = true;

    public bool finished = false;
    public Text uiText;


    [System.Serializable]   
    public class PictureLevel {
        public List<string> neededItems = new List<string>();
        public List<string> forbidenItems = new List<string>();
        public bool suc = false;
        public bool strict = false;
    }

    public List<PictureLevel> pictureLevels = new List<PictureLevel>();


    Photo _photo;

    public int index;

    private void Start()
    {
            uiText.text = "insert photo here";
    }

    private void OnTriggerEnter(Collider other)
    {
        if (finished)
            return;
        var photo = other.GetComponent<Photo>();
        if (photo != null && canTake) {
            canTake = false;
            CancelInvoke($"delay");
            TryPhoto(photo);
        }
    }

    void TryPhoto(Photo photo) {

        if (Interacter.Instance.holdingItem) {
            Interacter.Instance.holdingItem = false;
        }


        photo.able = false;
        photo.GetComponent<Rigidbody>().useGravity = false;
        photo.GetComponent<Rigidbody>().isKinematic = true;
        photo.transform.DORotateQuaternion(transform.rotation, 0.5f);
        photo.transform.DOMove(transform.position, 0.5f);

        uiText.text = "let me check...";


        _photo = photo;
        Invoke("CheckPhoto", 2f);
    }

    void CheckPhoto() {
        foreach (var pic in pictureLevels) {
            if (pic.suc)
                continue;
            bool confirm = true;
            foreach (var neededItem in pic.neededItems) {
                if (_photo.presentItems.Contains(neededItem)) {

                } else {
                    confirm = false;
                    uiText.text = "it's missing something";
                    goto fail;
                }
            }

            foreach (var forbidenItem in pic.forbidenItems) {
                if (_photo.presentItems.Contains(forbidenItem)) {
                    confirm = false;
                    uiText.text = "something shouldn't be here";
                    goto fail;
                }
            }

            
            if (confirm) {
                pic.suc = true;
                ConfirmPhoto(_photo);
                return;
            }
        }

        fail:
        RejectPhoto(_photo);
    }

    void NextLevel() {
        finished = true;

        Menu.Instance.NextLevel();
    }

    void RejectPhoto(Photo photo) {
        _photo = photo;
        image.color = Color.red;
        Invoke($"delay", 3f);   
    }
    void delay() {
        _photo.able = true;
        _photo.GetComponent<Rigidbody>().useGravity = true;
        _photo.GetComponent<Rigidbody>().isKinematic = false;
        _photo.GetComponent<Rigidbody>().AddForce(-transform.forward * 200F);
        image.color = Color.white;

        CancelInvoke($"delay2");
        Invoke($"delay2", 2f);
    }
        
    void delay2() {
        canTake = true;
    }

    void ConfirmPhoto(Photo photo) {
        CancelInvoke($"delay");
        
        image.color = Color.green;

        uiText.text = "great !";

        Invoke("CheckWin", 2f);
    }

    void CheckWin() {
        bool suc = true;
        foreach (var item in pictureLevels) {
            if (!item.suc)
                suc = false;
        }

        if (suc) {
            NextLevel();
        } else {
            _photo.transform.DOScale(0f, 0.5f).SetEase(Ease.InBounce);
            image.color = Color.white;

            uiText.text = "another photo please";
            canTake = true;
        }

    }
}
