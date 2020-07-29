using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorUnlock : MonoBehaviour
{
    ///public Animator lockedDoor;
    public GameObject door;
    public bool box1 = false;
    public bool box2 = false;
    public bool box3 = false;


    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Box")
        {
            box1 = true;
            Debug.Log(" Box 1 Placed");
        }

        if (other.gameObject.tag == "Box2")
        {
            box2 = true;
            Debug.Log(" Box 2 Placed");
        }

        if (other.gameObject.tag == "Box3")
        {
             box3 = true;
             Debug.Log(" Box 3 Placed");
        }
        else
        {
            Debug.Log(" Keys Required ");
        }

        if (box1 == true )
        {
            if(box2 == true)
            {
                if(box3 == true)
                {
                    DoorOpen();
                }
            }           
            
        }
                          
        
    }

    void DoorOpen()
    {

        Destroy(door);
         Debug.Log(" Key Set... Now Unlocking ");
       
    }
}
