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

    private void Awake()
    {
        Instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        HideReticleDelay();   
    }

    bool inside = false;

    // Update is called once per frame
    void Update()
    {
        if (!holdingItem )
        {
            if (CameraBehavior.Instance.takingPhoto) {
                if (inside)
                    HideReticle();
                return;

            }
            var ray = new Ray(transform.position, transform.forward);
            var hit = new RaycastHit();
            if (Physics.Raycast(ray, out hit, 100f))
            {
                var interactable = hit.collider.GetComponent<Interactable>();
                if (interactable != null && interactable.able)
                {
                    if (!inside) {
                        inside = true;
                        ShowReticle();
                    }
                    if (Input.GetMouseButtonDown(0)) {
                        PickUpItem(interactable.transform);
                    }
                } else {
                    if (inside) {
                        HideReticle();
                        inside = false;
                    }
                }
            } else if (inside) {
                HideReticle();
                inside = false;
            }
        } else
        {
            currentItem.position = Vector3.Lerp(currentItem.position, target.position, speed * Time.deltaTime);
            currentItem.rotation = Quaternion.Lerp(currentItem.rotation, target.rotation, speed * Time.deltaTime);

            if (Input.GetMouseButtonDown(0)) {
                holdingItem = false;
                currentItem.GetComponent<Rigidbody>().isKinematic = false;
                currentItem.GetComponent<Rigidbody>().AddForce(transform.forward * force);
            }
        }
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
        HideReticle();
        inside = false;

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
