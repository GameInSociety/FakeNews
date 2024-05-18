using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class Vortex : MonoBehaviour
{
    public Image image;
    private bool canTake = true;

    public bool strict = false;

    public List<Photo.Info> infos = new List<Photo.Info>();

    Photo _photo;

    private void OnTriggerEnter(Collider other)
    {
        var photo = other.GetComponent<Photo>();
        if (photo != null && canTake) {
            canTake = false;
            Debug.Log($"try photo");
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
        int count = 0;
        foreach (var info in infos) {
            if (_photo.infos.Find(x => x.name == info.name) != null) {
                ++count;
            }
        }

        if (strict) {
            if (infos.Count == count) {
                ConfirmPhoto(_photo);
            } else {
                RejectPhoto(_photo);
            }
        } else {
            if (count >= infos.Count) {
                ConfirmPhoto(_photo);
            } else {
                RejectPhoto(_photo);
            }
        }
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
    }
}
