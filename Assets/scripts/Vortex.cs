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

    public string[] texts;

    public Text uiText;

    public bool strict = false;

    public class PictureLevel {
        public List<Photo.Info> infos = new List<Photo.Info>();
        public bool suc = false;
    }

    public List<PictureLevel> pictureLevels = new List<PictureLevel>();


    Photo _photo;

    public int index;

    private void Start()
    {
        uiText.text = texts[index];
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

        _photo = photo;
        Invoke("CheckPhoto", 2f);
    }

    void CheckPhoto() {
        foreach (var pic in pictureLevels) {
            int count = 0;
            foreach (var info in pic.infos) {
                if (_photo.infos.Find(x => x.name == info.name) != null)
                    ++count;
            }

            if (count >= pic.infos.Count) {
                ConfirmPhoto(_photo);
                return;
            }
        }

        
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
        _photo.GetComponent<Rigidbody>().AddForce(transform.forward * 200F);
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
        }

    }
}
