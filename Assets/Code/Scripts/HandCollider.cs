using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class HandCollider : MonoBehaviour
{
    [HideInInspector] public GameObject grabbedObj;
    [HideInInspector] public Rigidbody rb;
    [HideInInspector] public bool isGrabbing;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        isGrabbing = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        // Check if the collider's GameObject is on the "Items" layer
        if (other.gameObject.layer == LayerMask.NameToLayer("Items"))
        {
            grabbedObj = other.gameObject;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // Check if the collider's GameObject is on the "Items" layer
        if (other.gameObject.layer == LayerMask.NameToLayer("Items"))
        {
            grabbedObj = null;
        }
    }
}
