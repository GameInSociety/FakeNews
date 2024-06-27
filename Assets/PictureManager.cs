using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PictureManager : MonoBehaviour
{
    public static PictureManager Instance;

    public List<Texture2D> photos = new List<Texture2D>();  

    private void Awake() {
        Instance = this;
    }
}
