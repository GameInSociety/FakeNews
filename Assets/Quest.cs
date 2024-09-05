using System.Collections;
using System.Collections.Generic;
using System.Security;
using UnityEngine;

[System.Serializable]
public class Quest {

    public enum State {
        None,
        Doing,
        Done,
    }

    public State currentState;

    public string id;
    public string name;
    public string article;
    public string clue;
    public string dialogue_start;
    public string dialogue_end;
    public List<string> item_ids = new List<string>();
    public List<int> item_score = new List<int>();
}