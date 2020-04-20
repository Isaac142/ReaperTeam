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
            itemMovement.canHold = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            itemMovement.player = null;
            itemMovement.canHold = false;

            if (itemMovement.transform.GetComponent<Renderer>() != null)
                itemMovement.transform.GetComponent<Renderer>().material.DisableKeyword("_EMISSION");
        }
    }
}
