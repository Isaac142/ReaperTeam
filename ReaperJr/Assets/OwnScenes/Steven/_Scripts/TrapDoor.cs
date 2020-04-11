using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrapDoor : MonoBehaviour
{

    public Animator trapDoorAnimation;



    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            trapDoorAnimation.SetBool("TrapTriggered", true);
            Debug.Log("Trap Door Active");
        }
    }
}
