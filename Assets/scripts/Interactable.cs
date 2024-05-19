using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactable : MonoBehaviour {
    public bool able = true;
    public Quaternion initRot;
    public bool turnToCam = false;

    public bool nextLevel = false;

    // Start is called before the first frame update
    void Start() {
        initRot = transform.rotation;
        Rigidbody r = GetComponent<Rigidbody>();
        if (r == null)
            r = gameObject.AddComponent<Rigidbody>();

        CameraBehavior.Instance.onTakePicture += Test;
    }

    // Update is called once per frame
    void Update() {

    }

    void Test() {
        var v = Camera.main.WorldToViewportPoint(transform.position);
        if ( v.x > 0 && v.x < 1f && v.y > 0f && v.y < 1f && v.z > 0f) {
            Photo.current.presentItems.Add(name);
        }
    }
}
