using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallingObjects : MonoBehaviour
{
    Rigidbody rb;

    public GameObject block;

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
            BlockDestroy();
            Debug.Log(" activating destroy box ");
           
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
    IEnumerator BlockDestroy()
    {
        yield return new WaitForSeconds(3);
        block.gameObject.SetActive(false);
        Debug.Log(" Destroying block in 3... 2... 1...");
    }

}
