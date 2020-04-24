using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorLocking : MonoBehaviour
{
    public GameObject Door;
    // Start is called before the first frame update
    void Start()
    {
        Door.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void OnTriggerExit(Collider other)
    {
        if(other.gameObject.tag == "Player")
        {
            Door.gameObject.SetActive(true);
        }
    }
}
