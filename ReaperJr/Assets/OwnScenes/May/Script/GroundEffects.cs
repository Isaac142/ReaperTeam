using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundEffects : MonoBehaviour
{
    public float effectFactor = 1f;
    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            other.GetComponent<Rigidbody>().mass += effectFactor; //used for addForce type of character control

            //if controlled by transform.translate
            //other.GetComponent<"characterControlScript">."speed" -=effectFactor;


        //used on slipery surface
            //other.GetComponent<Rigidbody>().drag -= effectFactor;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            other.GetComponent<Rigidbody>().mass -= effectFactor;
        }
    }
}
