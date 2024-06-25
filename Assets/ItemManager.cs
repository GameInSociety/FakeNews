using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemManager : MonoBehaviour {
    public static ItemManager Instance;

    private void Awake() {
        Instance = this;
    }

    public List<Item> items = new List<Item>();

    public void AddItem(Item item) {
        items.Add(item);
    }

    public void GetItem(string _id) {

    }
}
