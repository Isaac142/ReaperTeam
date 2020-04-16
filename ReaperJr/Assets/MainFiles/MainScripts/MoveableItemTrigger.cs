using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveableItemTrigger : MonoBehaviour
{
    public ItemMovement itemMovement;

    public void Awake()
    {
        itemMovement = transform.GetComponentInParent<ItemMovement>();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            itemMovement.player = other.gameObject;

            if(transform.parent.GetComponent<Renderer>() != null)
                transform.parent.GetComponent<Renderer>().material.EnableKeyword("_EMISSION");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            itemMovement.player = null;

            if (transform.parent.GetComponent<Renderer>() != null)
                transform.parent.GetComponent<Renderer>().material.DisableKeyword("_EMISSION");
        }
    }
}
