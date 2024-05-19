using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
public class CameraBehavior : MonoBehaviour
{
    public static CameraBehavior Instance;
    public bool takingPhoto = false;
    bool tookPhoto = false;

    public delegate void OnTakePicture();
    public OnTakePicture onTakePicture;

    public AudioSource source;

    public int resWidth = 2550;
    public int resHeight = 3300;

    private bool takeHiResShot = false;

    GameObject photo_instance;
    public float photo_force = 50f;

    public Transform photo_target;
    public GameObject photo_prefab;

    public Animator _animator;

    Texture2D texture;

    private void Awake()
    {
        Instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(1) && !Interacter.Instance.holdingItem) {
            if (takingPhoto) {
                Camera_Exit();
            } else {
                Camera_Start();
            }
        }

        if (takingPhoto) {
            Camera_Update();
        }


    }

    void Camera_Start() {
        takingPhoto = true;
        _animator.SetTrigger("pull out");
        tookPhoto = false;
    }

    void Camera_Update() {
        if (Input.GetMouseButtonDown(0) && !tookPhoto)
            TakePicture();
    }

    void Camera_Exit() {
        takingPhoto = false;
        _animator.SetTrigger("exit");
    }

    void TakePicture() {
        takeHiResShot = true;
        source.Play();
    }

    void LateUpdate() {
        if (takeHiResShot) {
            tookPhoto = true;
            texture = GetTextureFromCamera(Camera.main);
            takeHiResShot = false;
            Invoke("delay", 0f);
        }
    }

    void delay() {
        Camera_Exit();
        photo_instance = Instantiate(photo_prefab, null);
        photo_instance.GetComponentsInChildren<MeshRenderer>()[1].material.mainTexture = texture;

        Interacter.Instance.PickUpItem(photo_instance.transform);

        if (onTakePicture != null) {
            onTakePicture();
        }
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
