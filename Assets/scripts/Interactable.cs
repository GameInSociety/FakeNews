using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactable : MonoBehaviour {

    public string id = "";

    public Item _linkedItem;

    public bool able = true;
    public Quaternion initRot;
    public bool turnToCam = false;

    public bool nextLevel = false;

    // Start is called before the first frame update
    void Start() {
        initRot = transform.rotation;
        Rigidbody r = GetComponent<Rigidbody>();
        if (r == null)
            r = gameObject.AddComponent<Rigidbody>();

        CameraBehavior.Instance.onTakePicture += Test;
        SetId();
    }

    public virtual void SetId() {
        if (string.IsNullOrEmpty(id))
            return;
        _linkedItem = ItemManager.Instance.items.Find(x => x.id == id);
        if (_linkedItem == null) {
            Debug.Log($"found no item for interactable with id : {id}");
        } else {

        }
    }

    public virtual void PickUp() {
        Interacter.Instance.holdingItem = true;
        Interacter.Instance.currentInteractable = this;
        GetComponent<Rigidbody>().isKinematic = true;

        foreach (var b in GetComponentsInChildren<Collider>()) {
            b.enabled = false;
        }

        transform.SetParent(Interacter.Instance.rotate_Box);
    }

    // Update is called once per frame
    void Update() {

    }

    void Test() {
        if ( string.IsNullOrEmpty( id ) ) {
            Debug.Log($"skipping : {name}, no id");
            return;
        }

        var v = Camera.main.WorldToViewportPoint(transform.position);
        if ( v.x > 0 && v.x < 1f && v.y > 0f && v.y < 1f && v.z > 0f) {
            Photo.current.AddItem(_linkedItem);
        }

    }
}





public class Enemy {
    public string name = "";
    public int hitPoints = 10;

    public void Attack() {

    }
}

public class Archer : Enemy {

}

public class Warrior : 