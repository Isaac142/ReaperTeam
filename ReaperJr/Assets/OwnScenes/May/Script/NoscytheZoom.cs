using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoscytheZoom : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
            other.GetComponent<ThrowableScythe>().enabled = false;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
            other.GetComponent<ThrowableScythe>().enabled = true;
    }
}
