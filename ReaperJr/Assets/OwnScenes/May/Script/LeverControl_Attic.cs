using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeverControl_Attic : ReaperJr
{
    private AtticDoorSwitch controller;
    public float clickDist = 1.5f;
    private float playerDist;

    private void Start()
    {
        controller = GetComponentInParent<AtticDoorSwitch>();
    }

    void Update()
    {
        if (!controller.doorLocked)
            return;

        if (controller.playerApproach && controller.allKeysIn)
        {
            playerDist = Vector3.Distance(_PLAYER.transform.position, this.transform.position);
        }
        
        if (controller.playerApproach)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                _UI.SetHintPanel();
                
                if (hit.transform == this.transform)
                {
                    if (controller.allKeysIn)
                    {
                        if (playerDist <= clickDist)
                            GameEvents.ReportHintShown(HintForActions.SWITCH);

                        else
                            GameEvents.ReportHintShown(HintForActions.DISTANCEREQUIRED);
                    }

                    else
                        GameEvents.ReportHintShown(HintForActions.REQUIRKEY);
                }
            }

            if (playerDist <= clickDist && controller.allKeysIn)
            {
                if (Input.GetMouseButtonDown(1))
                {
                    controller.switchActivated = true;
                    controller.anim.SetBool("SwitchOn", false);
                    controller.FinalPos();
                }
            }
        }
    }
}
