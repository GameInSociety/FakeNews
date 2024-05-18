using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Interacter : MonoBehaviour
{
    public static Interacter Instance;

    public bool holdingItem = false;
    public Transform currentItem;
    public float speed = 1f;
    public Transform target;
    public float force = 100f;

    public GameObject reticle_group;
    public CanvasGroup reticle_cg;
    bool selected = false;

    private void Awake()
    {
        Instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        HideReticleDelay();   
    }


    // Update is called once per frame
    void Update()
    {
        if (holdingItem) {
            currentItem.position = Vector3.Lerp(currentItem.position, target.position, speed * Time.deltaTime);
            currentItem.rotation = Quaternion.Lerp(currentItem.rotation, target.rotation, speed * Time.deltaTime);

            if (Input.GetMouseButtonDown(0)) {
                holdingItem = false;
                currentItem.GetComponent<Rigidbody>().isKinematic = false;
                currentItem.GetComponent<Rigidbody>().AddForce(transform.forward * force);
            }
            return;
        }

        if (CameraBehavior.Instance.takingPhoto) {
            Deselect();
            return;
        }

        var ray = new Ray(transform.position, transform.forward);
        var hit = new RaycastHit();
        if (Physics.Raycast(ray, out hit, 100f)) {
            var interactable = hit.collider.GetComponent<Interactable>();
            if (interactable != null && interactable.able) {
                Select();
                if (Input.GetMouseButtonDown(0))
                    PickUpItem(interactable.transform);
            } else {
                Deselect();
            }
            return;
        }
        Deselect();
    }

    void Select() {
        if (selected)
            return;
        selected = true;
        ShowReticle();
        gameObject.layer = 7;
        Debug.Log($"selcte");
    }

    void Deselect() {
        if (!selected)
            return;
        selected = false;
        HideReticle();
        gameObject.layer = 0;
        Debug.Log($"deselect");

    }

    void ShowReticle() {
        reticle_group.SetActive(true);
        reticle_cg.alpha = 0f;
        reticle_cg.DOKill();
        reticle_cg.DOFade(1f, 0.2f);
    }
    void HideReticle() {
        reticle_cg.DOKill();
        reticle_cg.DOFade(0f, 0.2f);
        CancelInvoke("HideReticleDelay");
        Invoke("HideReticleDelay", 0.3f);
    }

    void HideReticleDelay() {
        reticle_group.SetActive(false);
    }
    public void PickUpItem(Transform interactable) {
        Deselect();

        holdingItem = true;
        currentItem = interactable.transform;
        currentItem.GetComponent<Rigidbody>().isKinematic = true;

        var photo = interactable.GetComponent<Photo>();
        if (photo != null) {
            Photo.current = photo;
            Debug.Log($"Current Photo : {photo.name}");
        }
    }
}
