using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemMovement : MonoBehaviour
{    
    public float mass;
    public ItemMovement itemMovement;
    public bool isHolding;
    public GameObject player;
    PlayerMovement PM;

    private void Start()
    {
        itemMovement.enabled = false;
        isHolding = false;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            isHolding = true;

            player.GetComponent<Rigidbody>().mass += mass;
            gameObject.transform.parent = player.transform;
        }

        if (Input.GetKeyDown(KeyCode.R) && isHolding == true)
        {
            isHolding = false;
            transform.parent = null;
        }

        if (transform.parent != null && transform.parent.tag == "Player")
        {
                itemMovement.enabled = true;
        }
    }
}
