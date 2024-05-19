using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Interacter : MonoBehaviour
{
    public static Interacter Instance;

    public bool holdingItem = false;
    public Interactable currentItem;
    public float speed = 1f;
    public Transform target;
    public float force = 100f;

    public GameObject reticle_group;
    public CanvasGroup reticle_cg;
    bool selected = false;
    public Interactable overring;


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
            currentItem.transform.position = Vector3.Lerp(currentItem.transform.position, target.position, speed * Time.deltaTime);

            if (!currentItem.turnToCam) {
                currentItem.transform.rotation = Quaternion.Lerp(currentItem.transform.rotation, currentItem.initRot, speed * Time.deltaTime);
            } else {
                currentItem.transform.rotation = Quaternion.Lerp(currentItem.transform.rotation, target.rotation    , speed * Time.deltaTime);
            }

            if (Input.GetMouseButtonDown(0)) {
                holdingItem = false;
                foreach (var b in currentItem.GetComponentsInChildren<Collider>()) {
                    b.enabled = true;
                }
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
                Select(interactable);
                if (Input.GetMouseButtonDown(0))
                    PickUpItem(interactable);
            } else {
                Deselect();
            }
            return;
        }
        Deselect();
    }

    void Select(Interactable i) {
        if (selected)
            return;
        overring = i;
        i.gameObject.layer = 7;
        selected = true;
        ShowReticle();
    }

    void Deselect() {
        if (!selected)
            return;
        if (overring != null)
            overring.gameObject.layer = 0;
        selected = false;
        HideReticle();
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
    public void PickUpItem(Interactable interactable) {
        Deselect();

        if (interactable.nextLevel) {
            interactable.gameObject.SetActive(false);   
            Menu.Instance.StartLevel();
            return;
        }

        holdingItem = true;
        currentItem = interactable;
        currentItem.GetComponent<Rigidbody>().isKinematic = true;

        foreach (var b in currentItem.GetComponentsInChildren<Collider>()) {
            b.enabled = false;
        }

        var photo = interactable.GetComponent<Photo>();
        if (photo != null) {
            Photo.current = photo;
            Debug.Log($"Current Photo : {photo.name}");
        }
    }
}
