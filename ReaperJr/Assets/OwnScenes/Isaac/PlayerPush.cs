using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPush : MonoBehaviour
{

    public float rayDistance = 1f;
    public LayerMask boxMask;

    public GameObject rayStart;

    public GameObject box;

    public Rigidbody player;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

        //RaycastHit hit;
        //Physics.Raycast(transform.position, Vector3.right * transform.localScale.x, rayDistance, boxMask);
    }

    private void FixedUpdate()
    {
        RaycastHit hit;

        if (Physics.Raycast(rayStart.transform.position, rayStart.transform.forward, out hit, 100))
        {
            Debug.DrawLine(rayStart.transform.position, hit.point);
        }
    }

    void RayCastActivator()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            RaycastHit hit;

            if (Physics.Raycast(rayStart.transform.position, rayStart.transform.forward, out hit, 100))
            {
                Debug.DrawLine(rayStart.transform.position, hit.point);
                print(hit.collider.name);
            }
        }
    }

    void ConnectingWires()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            //Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(rayStart.transform.position, rayStart.transform.forward, out hit, 100))
            {
                print(hit.collider.name);

                if (hit.collider.CompareTag("Box"))
                {
                    box.GetComponent<FixedJoint>().connectedBody = player.GetComponent<Rigidbody>();
                }
            }

            //Debug.DrawLine(ray.origin, hit.point);
        }
    }
}
