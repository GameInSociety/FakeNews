using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC : MonoBehaviour
{
    public enum State {
        none,
        sitting,
        walkingAround,
        stopping,
    }

    public Animator animator;

    public State state = State.none;

    public float state_Timer = 0f;
    public float state_ExitTime = 0f;

    public float sitting_minTime = 10f;
    public float sitting_maxTime = 10f;

    public float walking_minTime = 10f;
    public float walking_maxTime = 10f;
    public float walking_speed = 1f;

    public float stopping_minTime = 10f;
    public float stopping_maxTime = 10f;

    public float turnSpeed = 1f;

    float timer = 0f;
    public delegate void OnUpdateState();
    public OnUpdateState onUpdateState;

    public int path_Index = 0;
    public Transform[] path_Nodes;
    public Transform path_Parent;
    public float path_DistanceToNextNode;
    public bool path_ShowGizmo = false;
    public bool path_Backwards = false;

    // Start is called before the first frame update
    void Start()
    {
        var ts = path_Parent.GetComponentsInChildren<Transform>();
        path_Nodes = new Transform[ts.Length - 1];

        for (int i = 0; i < path_Nodes.Length; i++) {
            path_Nodes[i] = ts[i + 1];
        }
        StartState(State.walkingAround);
    }

    // Update is called once per frame
    void Update()
    {
        UpdateState();

    }

    public void StartState(State state) {
        state_Timer = 0f;
        switch (state) {
            case State.sitting:
                Sitting_Start();
                onUpdateState = Sitting_Update;
                break;
            case State.walkingAround:
                Walking_Start();
                onUpdateState = Walking_Update;
                break;
            case State.stopping:
                Stopping_Start();
                onUpdateState = Stopping_Update;
                break;
        }
    }

    public void UpdateState() {
        if (onUpdateState != null) {
            onUpdateState();
            state_Timer += Time.deltaTime;
        }
    }

    public void ExitState(State state) {

    }

    public void Sitting_Start() {
        animator.SetTrigger("sit");
    }

    public void Sitting_Update() {

    }

    public void Sitting_Stop() {

    }

    public void Walking_Start() {
        animator.SetTrigger("walk");
        path_Backwards = false;
    }

    public void Walking_Update() {

        var nextNode = path_Nodes[path_Index];
        var dir = nextNode.position - transform.position;
        transform.Translate(dir.normalized * walking_speed * Time.deltaTime, Space.World);
        transform.forward = Vector3.Lerp(transform.forward, dir.normalized, turnSpeed * Time.deltaTime);

        if ( dir.magnitude < path_DistanceToNextNode) {
           if (path_Backwards) {
                path_Index--;
                if ( path_Index == -1) {
                    Debug.Log("sitting");
                    StartState(State.sitting);
                }
            } else {
                path_Index++;
                if (path_Index == path_Nodes.Length) {
                    path_Index--;
                    Debug.Log($"going backwards");
                    path_Backwards = true;
                }
            }
        } else {
        }
    }

    public void Walking_Stop() { }

    public void Stopping_Start() {

    }

    public void Stopping_Update() {


    }

    public void Stopping_Stop() {

    }

    private void OnDrawGizmos() {
        for (int i = 0; i < path_Nodes.Length; i++) {
            Gizmos.DrawSphere(path_Nodes[i].position, 0.2f);

            if ( i < path_Nodes.Length - 1 ) {
                Gizmos.DrawLine(path_Nodes[i].position, path_Nodes[i + 1].position);
            }
        }
    }

    public void BuildPath() {
        var ts = path_Parent.GetComponentsInChildren<Transform>();
        path_Nodes = new Transform[ts.Length - 1];

        for (int i = 0; i < path_Nodes.Length; i++) {
            path_Nodes[i] = ts[i + 1];
        }
    }

}

