using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookHead : MonoBehaviour
{
    public Transform target;
    public Transform head_Transform;
    public Transform tmp_target;


    public float lerpSpeed = 1f;


    private void Start() {
        tmp_target = new GameObject().transform;
        tmp_target.parent = transform;
        tmp_target.name = "TmpTarget";
        if (target != null)
            SetTarget(target);
    }

    private void LateUpdate() {
        if ( target == null )
            return;
        var dir = (target.position-head_Transform.position).normalized;
        tmp_target.position = Vector3.Lerp(tmp_target.position, target.position, lerpSpeed * Time.deltaTime);
        head_Transform.forward = dir;
    }

    public void SetTarget(Transform t) {
        target = t;
    }
}
