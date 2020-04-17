using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorUnlock : MonoBehaviour
{
    public Animator lockedDoor;

     

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Box")
        {
            lockedDoor.SetBool("LockedDoor", true);

            Debug.Log(" Key Set... Now Unlocking ");
        }
        else
        {
            Debug.Log(" Key Required ");
        }
    }
}
