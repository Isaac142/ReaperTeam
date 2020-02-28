using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Teleport_01 : MonoBehaviour
{

    public float distance = 10f;

    private void Start()
    {
        
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            TeleportForward();
        }
    }

    void TeleportForward()
    {
        RaycastHit solid;
        Vector3 destination = transform.position + transform.forward * distance;
        if (Physics.Linecast(transform.position, destination, out solid))
        {
            destination = transform.position + transform.forward * (solid.distance - 1f);
        }
            
        if(Physics.Raycast(destination, -Vector3.up, out solid))
        {
            destination.y = 0.5f;
            destination = solid.point;
            transform.position = destination;
        }
    }
}
