using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatormParent : MonoBehaviour
{
    public bool isParented;

    void Start()
    {
        isParented = false;
    }
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            isParented = true;
            other.gameObject.transform.SetParent(transform);
            Debug.Log(" Player Parented");
        }
    }
    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            isParented = false;
            other.gameObject.transform.SetParent(null);
            Debug.Log(" Player Jumped off");
        }
    }
}
