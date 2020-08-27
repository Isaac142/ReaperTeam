﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveableItemTrigger : ReaperJr
{
    public ItemMovement itemMovement;

    public void Start()
    {
        itemMovement = transform.GetComponentInParent<ItemMovement>();
        GetComponent<BoxCollider>().isTrigger = true;
        this.gameObject.layer = 2; // set into ignore raycast layer.
    }
    private void OnTriggerEnter(Collider other)
    {
        if (itemMovement != null && !_GAME.playerIn)
        {
                if (other.tag == "Player")
                {
                    itemMovement.playerIn = true;
                    _GAME.playerIn = true;
                
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (itemMovement != null && itemMovement.playerIn)
        {
            if (other.tag == "Player")
            {
                itemMovement.playerIn = false;
                _GAME.playerIn = false;
            }
        }
    }
}
