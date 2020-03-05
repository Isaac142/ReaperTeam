using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatormParent : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            other.gameObject.transform.SetParent(transform);
            Debug.Log(" Player Parented");
        }
    }
    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            other.gameObject.transform.SetParent(null);
            Debug.Log(" Player Jumped off");
        }
    }
}
