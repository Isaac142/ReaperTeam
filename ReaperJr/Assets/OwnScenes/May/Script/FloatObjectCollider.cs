using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatObjectCollider : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if(other.GetComponent<Rigidbody>() != null)
            GetComponentInParent<Rigidbody>().mass += other.GetComponent<Rigidbody>().mass;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<Rigidbody>() != null)
            GetComponentInParent<Rigidbody>().mass -= other.GetComponent<Rigidbody>().mass;
    }
}
