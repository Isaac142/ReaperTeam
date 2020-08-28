﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheeseTrigger : ReaperJr
{
    public CheeseMovement itemMovement;

    public void Start()
    {
        itemMovement = transform.GetComponentInParent<CheeseMovement>();
        GetComponent<BoxCollider>().isTrigger = true;
        this.gameObject.layer = 2; // set into ignore raycast layer.
    }
    private void OnTriggerEnter(Collider other)
    {
        if (itemMovement != null)
        {
            if (other.tag == "Player")
            {
                itemMovement.playerIn = true;

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

            }
        }
    }
}
