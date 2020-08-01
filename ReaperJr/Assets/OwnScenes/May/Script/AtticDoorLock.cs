using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AtticDoorLock : ReaperJr
{
    public AtticDoorSwitch controller;
    public GameObject door;
    // Start is called before the first frame update
    
    private void Update()
    {
        if (controller.doorLocked && !door.activeSelf)
            door.SetActive(true);
        else if (!controller.doorLocked && door.activeSelf)
            door.SetActive(false);
        else if (!controller.doorLocked && controller.switchActivated)
            Destroy(this.gameObject);
        else
            return;
    }

    // Update is called once per frame
    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            if (!controller.doorLocked)
            {
                door.gameObject.SetActive(true);
                controller.doorLocked = true;
                controller.anim.SetBool("SwitchOn", true);
                _UI.SetKeyItemPanel(controller.keyItems);
            }
        }
    }
}

