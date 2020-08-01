using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AtticDoorSwitchCollider : ReaperJr
{
    private AtticDoorSwitch controller;
    // Start is called before the first frame update
    void Start()
    {
        controller = GetComponentInParent<AtticDoorSwitch>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
            controller.playerApproach = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            controller.playerApproach = false;
            _UI.SetHintPanel();
        }
    }
}
