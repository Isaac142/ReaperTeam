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
            itemMovement.playerIn = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            itemMovement.player = null;
            itemMovement.playerIn = false;
            itemMovement.canHold = false;

            if (itemMovement.transform.GetComponentInParent<Renderer>() != null)
                itemMovement.transform.GetComponentInParent<Renderer>().material.DisableKeyword("_EMISSION");
        }
    }
}
