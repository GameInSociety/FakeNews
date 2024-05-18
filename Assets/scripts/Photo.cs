using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Photo : Interactable
{
    public static Photo current;
    public List<Info> infos;

    [System.Serializable]
    public class Info {
        public string name;
    }
}
