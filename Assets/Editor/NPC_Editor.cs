
using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(NPC))]
public class NPC_Editor: Editor {
    public override void OnInspectorGUI() {

        base.OnInspectorGUI();
        var myTarget = (NPC)target;

        if (GUILayout.Button("Reset Path")) {
            myTarget.BuildPath();
        }
    }
}