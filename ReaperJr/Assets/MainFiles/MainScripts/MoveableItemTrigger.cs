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
            itemMovement.enabled = true;
            itemMovement.player = other.gameObject;

            if (itemMovement.objectRB != null)
                itemMovement.objectRB.isKinematic = true;

            if(transform.parent.GetComponent<Renderer>() != null)
                transform.parent.GetComponent<Renderer>().material.EnableKeyword("_EMISSION");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            if (itemMovement.isHolding != true)
            {
                itemMovement.enabled = false;
                itemMovement.player = other.gameObject;

                if (itemMovement.objectRB != null)
                    itemMovement.objectRB.isKinematic = false;

                if (transform.parent.GetComponent<Renderer>() != null)
                    transform.parent.GetComponent<Renderer>().material.DisableKeyword("_EMISSION");
            }
        }
    }
}
