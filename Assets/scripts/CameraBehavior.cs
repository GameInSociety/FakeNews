using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;
using UnityEngine.Rendering;

public class CameraBehavior : MonoBehaviour
{
    public static CameraBehavior Instance;

    // components
    public Animator _animator;
    public AudioSource source;

    // state
    public bool takingPhoto = false;

    /// <summary>
    ///  deletgates
    /// </summary>
    public delegate void OnTakePicture();
    public OnTakePicture onTakePicture;

    // photo
    GameObject photo_instance;
    public float photo_force = 50f;
    public Transform photo_target;
    public GameObject photo_prefab;

    [SerializeField] Camera LinkedCamera;
    [SerializeField] RenderTexture CameraRT;
    Texture2D LastImage;

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
    }

    void Camera_Update() {
        if (Input.GetMouseButtonDown(0))
            TakePicture();
    }

    void Camera_Exit() {
        takingPhoto = false;
        _animator.SetTrigger("exit");
    }

    void TakePicture() {
        source.Play();

        //// transfer the texture - this is GPU side only
        //Graphics.CopyTexture(CameraRT, LastImage);

        AsyncGPUReadback.Request(CameraRT, 0, (AsyncGPUReadbackRequest action) => {
            // create texture
            LastImage = new Texture2D(CameraRT.width, CameraRT.height,
                                      CameraRT.graphicsFormat,
                                      UnityEngine.Experimental.Rendering.TextureCreationFlags.None);
            LastImage.SetPixelData(action.GetData<byte>(), 0);
            LastImage.Apply();

            // save the image to file
            var currentTime = System.DateTime.Now;
            string fileName = $"Shot_{currentTime:yyyyMMdd_HHmmss}_photo.jpeg";
            fileName = System.IO.Path.Combine(Application.persistentDataPath, fileName);
            System.IO.File.WriteAllBytes(fileName, LastImage.EncodeToJPG());

            // show the last image
            PullOutPhoto();
        });
    }

    void PullOutPhoto() {
        Camera_Exit();
        photo_instance = Instantiate(photo_prefab, null);
        photo_instance.GetComponent<Photo>().rend.material.mainTexture = LastImage;
        PictureManager.Instance.photos.Add(LastImage);


        Interacter.Instance.PickUpItem(photo_instance.GetComponent<Interactable>());

        if (onTakePicture != null) {
            onTakePicture();
        }

        Photo.current.SetId();
    }
}
