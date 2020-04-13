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
        if(Input.GetMouseButtonDown(1) && !GameManager.Instance.scytheEquiped)
        {
            isHolding  =  !isHolding;
        }

        if (Input.GetMouseButtonDown(1) && isHolding)
        {
            player.GetComponent<Rigidbody>().mass += mass;
            gameObject.transform.parent = player.transform;
        }

        if (Input.GetMouseButtonDown(1) && !isHolding)
        {
            player.GetComponent<Rigidbody>().mass -= mass;
            transform.parent = null;
        }

        if (transform.parent != null && transform.parent.tag == "Player")
        {
                itemMovement.enabled = true;
        }
        GameManager.Instance.isHolding = isHolding;
    }
}
