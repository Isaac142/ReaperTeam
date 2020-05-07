using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class wallrenderer : MonoBehaviour
{
    public Renderer wall;
    public Renderer door;
    // Start is called before the first frame update
    void Start()
    {
        wall.gameObject.GetComponent<Renderer>();
        door.gameObject.GetComponent<Renderer>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            wall.GetComponent<Renderer>().enabled = false;
            door.GetComponent<Renderer>().enabled = false;
            Debug.Log(" Player entered the room");
        }
    }
    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            wall.GetComponent<Renderer>().enabled = true;
            door.GetComponent<Renderer>().enabled = true;
            Debug.Log(" Player Left the room");
        }

    }
}
