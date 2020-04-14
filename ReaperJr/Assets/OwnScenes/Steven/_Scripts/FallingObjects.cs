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
            Debug.Log(" LOOK OUT!! ");
        }

        if (other.gameObject.tag == "Soul")
        {
            Destroy(other.gameObject,0.5f);
            Debug.Log(" Soul lost to the abyss ");
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
