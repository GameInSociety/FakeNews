using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
public class CameraBehavior : MonoBehaviour
{
    bool takingPhoto = false;
    bool holdingPhoto = false;

    bool tookPhoto = false;

    public int resWidth = 2550;
    public int resHeight = 3300;

    private bool takeHiResShot = false;

    GameObject photo_instance;
    public float photo_force = 50f;

    public Transform photo_target;
    public GameObject photo_prefab;

    public MeshRenderer test;

    public Animator _animator;

    Texture2D texture;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.C) && !holdingPhoto) {
            if (takingPhoto) {
                Camera_Exit();
            } else {
                Camera_Start();
            }
        }

        if (takingPhoto) {
            Camera_Update();
        }

        if (holdingPhoto && Input.GetKeyDown(KeyCode.T)) {
            ThrowPhoto();
        }


    }

    void ThrowPhoto() {
        photo_instance.GetComponent<Rigidbody>().isKinematic = false;
        photo_instance.GetComponent<Rigidbody>().useGravity = true;
        photo_instance.GetComponent<Rigidbody>().AddForce(transform.forward * photo_force);
        photo_instance.transform.parent = null;
        holdingPhoto = false;
    }

    void Camera_Start() {
        takingPhoto = true;
        _animator.SetTrigger("pull out");
        tookPhoto = false;
    }

    void Camera_Update() {
        if (Input.GetKeyDown(KeyCode.T) && !tookPhoto) {
            TakePicture();
        }
    }

    void Camera_Exit() {
        takingPhoto = false;
        _animator.SetTrigger("exit");

        
    }

    void TakePicture() {
        takeHiResShot = true;
    }

    void LateUpdate() {
        if (takeHiResShot) {
            tookPhoto = true;
            texture = GetTextureFromCamera(Camera.main);
            takeHiResShot = false;
            Invoke("delay", .5f);
        }
    }

    void delay() {

        Camera_Exit();
        photo_instance = Instantiate(photo_prefab, transform);
        photo_instance.GetComponentsInChildren<MeshRenderer>()[1].material.mainTexture = texture;
        photo_instance.transform.DOMove(photo_target.position, 0.5f);
        photo_instance.transform.DORotateQuaternion(photo_target.rotation, 0.5f);



        holdingPhoto = true;
    }

    private static Texture2D GetTextureFromCamera(Camera mCamera) {
        Rect rect = new Rect(0, 0, mCamera.pixelWidth, mCamera.pixelHeight);
        RenderTexture renderTexture = new RenderTexture(mCamera.pixelWidth, mCamera.pixelHeight, 24);
        Texture2D screenShot = new Texture2D(mCamera.pixelWidth, mCamera.pixelHeight, TextureFormat.RGBA32, false);

        mCamera.targetTexture = renderTexture;
        mCamera.Render();

        RenderTexture.active = renderTexture;

        screenShot.ReadPixels(rect, 0, 0);
        screenShot.Apply();


        mCamera.targetTexture = null;
        RenderTexture.active = null;
        return screenShot;
    }
}
