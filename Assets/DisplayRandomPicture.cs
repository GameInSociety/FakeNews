using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisplayRandomPicture : MonoBehaviour
{

    public float timer = 0f;
    public float rate = 0f;
    public float minRate = 5f;
    public float maxRate = 20f;

    public Material mat;
    public int matIndex = 0;

    // Start is called before the first frame update
    void Start()
    {
        mat = GetComponent<MeshRenderer>().materials[matIndex]; 
        SetRate();
    }

    void SetRate() {
        rate = Random.Range(minRate, maxRate);
        timer = 0f;
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;

        if ( timer>= rate) {

            if (PictureManager.Instance.photos.Count== 0) {
                SetRate();
                return;
            }

            var photo = PictureManager.Instance.photos[Random.Range(0, PictureManager.Instance.photos.Count)];

            mat.mainTexture = photo;
            SetRate();
        }
    }
}
