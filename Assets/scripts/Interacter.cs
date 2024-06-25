using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEditor.ShaderGraph.Internal;
using UnityEditor;

public class Interacter : MonoBehaviour
{
    public static Interacter Instance;

    public bool holdingItem = false;
    public Interactable currentInteractable;
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

    // item rotation
    public Transform rotate_Box;
    public Vector3 mousePosition;
    public Vector3 mousePos;
    public Vector3 prevPos;
    public Vector3 delta;
    public float rotateSpeed = 100f;
    public float distanceSpeed = 100f;



    // Update is called once per frame
    void Update()
    {
            target.Translate(Vector3.forward * distanceSpeed * Input.GetAxis("Mouse ScrollWheel") * Time.deltaTime);
        if (holdingItem) {
            currentInteractable.transform.position = Vector3.Lerp(currentInteractable.transform.position, target.position, speed * Time.deltaTime);


            /*if (!currentItem.turnToCam) {
                currentItem.transform.rotation = Quaternion.Lerp(currentItem.transform.rotation, currentItem.initRot, speed * Time.deltaTime);
            } else {
                currentItem.transform.rotation = Quaternion.Lerp(currentItem.transform.rotation, target.rotation    , speed * Time.deltaTime);
            }*/

            if (Input.GetMouseButtonDown(0)) {
                holdingItem = false;
                foreach (var b in currentInteractable.GetComponentsInChildren<Collider>()) {
                    b.enabled = true;
                }
                currentInteractable.GetComponent<Rigidbody>().isKinematic = false;
                currentInteractable.GetComponent<Rigidbody>().AddForce(transform.forward * force);
            }


            if (Input.GetMouseButton(1)) {
                FirstPersonController.instance.cameraCanMove = false;
                
                var cam = Camera.main;

                float rotX = Input.GetAxis("Mouse X") * rotateSpeed;
                float rotY = Input.GetAxis("Mouse Y") * rotateSpeed;

                Vector3 right = Vector3.Cross(cam.transform.up, currentInteractable.transform.position - cam.transform.position);
                Vector3 up = Vector3.Cross(currentInteractable.transform.position - cam.transform.position, right);
                currentInteractable.transform.rotation = Quaternion.AngleAxis(-rotX, up) * currentInteractable.transform.rotation;
                currentInteractable.transform.rotation = Quaternion.AngleAxis(rotY, right) * currentInteractable.transform.rotation;


            } else {
                FirstPersonController.instance.cameraCanMove = true;
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
        foreach (var item in i.GetComponentsInChildren<Transform>())
        {
            item.gameObject.layer = 7;
        }
        selected = true;
        ShowReticle();
    }

    void Deselect() {
        if (!selected)
            return;
        if (overring != null)
        {
            foreach (var item in overring.GetComponentsInChildren<Transform>())
            {
                item.gameObject.layer = 0;
            }
        }
        selected = false;
        HideReticle();

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
        interactable.PickUp();

        
    }
}
