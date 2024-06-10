using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StepSound : MonoBehaviour
{

    public AudioSource foorstepSource;
    public AudioClip[] clips;

    public FirstPersonController controller;

    public float footStepRate = 0.5f;
    float t = 0f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (controller.isWalking) {

            t += Time.deltaTime;
            if (t > footStepRate) {
                t = 0f;
                foorstepSource.clip = clips[Random.Range(0, clips.Length)];
                foorstepSource.Play();
            }
        } else {
            t = 0f;
        }
    }
}
