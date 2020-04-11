using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallingObjects : MonoBehaviour
{
    Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            rb.isKinematic = false;
            Debug.Log(" Items Dropping ");
        }

    }

    void OnCollisionEnter(Collision collider)
    {
        if (collider.gameObject.tag == "Player")
        {
            rb.isKinematic = false;
            Debug.Log(" Player Hit ");
        }
    }

}
