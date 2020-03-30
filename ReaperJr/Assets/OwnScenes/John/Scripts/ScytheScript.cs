using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScytheScript : MonoBehaviour
{
    public bool activated;

    private void OnCollisionEnter(Collision collision)
    {
        activated = false;
        GetComponent<Rigidbody>().isKinematic = true;
    }
}
