using Autodesk.Fbx;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    public Transform targetTransform;
    public float targetAngle;
    public float rotateSpeed = 10;

    private bool open = false;

    private Quaternion initRot;

    float timer = 0f;
    public float timeToClose = 2f;

    private void Start() {
        initRot = transform.localRotation;
    }

    private void Update() {
        
        if ( open) {
            timer += Time.deltaTime;
            if (timer >= timeToClose ) {
                open = false;
            }
            targetTransform.localRotation = Quaternion.Lerp(targetTransform.localRotation, Quaternion.Euler(Vector3.up * targetAngle), rotateSpeed * Time.deltaTime); ; ; ;
        } else {
            targetTransform.localRotation = Quaternion.Lerp(targetTransform.localRotation, Quaternion.identity, rotateSpeed * Time.deltaTime); ;
        }
    }

    public void Open() {
        open = true;
    }

    public void Close() {
        open = false;
    }

    private void OnTriggerStay(Collider other) {
        if ( other.tag == "Player" || other.tag == "NPC") {
            open = true;
            timer = 0f;
        }
    }
}
