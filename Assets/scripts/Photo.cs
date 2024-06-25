using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Photo : Interactable
{
    public static Photo current;
    public List<string> presentItems;
    public List<Item> items = new List<Item>();
    public MeshRenderer rend;

    public void AddItem(Item item) {
        items.Add(item);
    }

    public override void PickUp() {
        base.PickUp();
        Photo.current = this;

        
    }

    public override void SetId() {
        if (string.IsNullOrEmpty(id)) {
            items = items.OrderBy(x => x.id).ToList();
            foreach (var item in items) {
                id += $"{item.id}";
            }

            id = $"_photo{id}";
            Debug.Log($"setting id : {id}");
        }
    }

    public Texture2D GetTexture() {
        return rend.material.mainTexture as Texture2D;
    }
}
