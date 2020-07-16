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
            itemMovement.playerIn = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
            itemMovement.playerIn = false;
    }
}
