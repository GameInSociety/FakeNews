using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemManager : MonoBehaviour {
    private static ItemManager _instance;
    public static ItemManager Instance {
        get {
            if (_instance == null)
                _instance = FindFirstObjectByType<ItemManager>();
            return _instance;
        }
    }


    public List<Item> items = new List<Item>();

    public void AddItem(Item item) {
        items.Add(item);
    }

    public void GetItem(string _id) {

    }
}
