using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interacter : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        var ray = new Ray(transform.position, transform.forward);
        var hit = new RaycastHit();
        if ( Physics.Raycast(ray, out hit, 100f)) {
            var interactable = hit.collider.GetComponent<Interactable>();
            if (interactable!=null)
                Debug.Log($"look at {hit.collider.name}");
        }
    }
}
