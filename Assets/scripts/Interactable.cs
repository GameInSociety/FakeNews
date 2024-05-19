using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactable : MonoBehaviour {
    public Vector3 testV;
    public bool able = true;
    // Start is called before the first frame update
    void Start() {
        CameraBehavior.Instance.onTakePicture += Test;
    }

    // Update is called once per frame
    void Update() {

    }

    void Test() {
        var v = Camera.main.WorldToViewportPoint(transform.position);
        testV = v;
        if ( v.x > 0 && v.x < 1f && v.y > 0f && v.y <    1f && v.z > 0f) {
            var pi = new Photo.Info();
            pi.name = name;
            Photo.current.infos.Add(pi);
        }
    }
}
