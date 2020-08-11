using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeverControl_Attic : ReaperJr
{
    private AtticDoorSwitch controller;
    public float clickDist = 1.5f;
    private float playerDist;
    private Material mat;
    public GameObject particles;

    private void Start()
    {
        controller = GetComponentInParent<AtticDoorSwitch>();
        mat = GetComponentInChildren<Renderer>().material;
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

        if (controller.allKeysIn && !controller.switchActivated)
        {
            mat.EnableKeyword("_EMISSION");
            particles.SetActive(true);
        }
        else
        {
            mat.DisableKeyword("_EMISSION");
            particles.SetActive(false);
        }
    }


    private void OnMouseEnter()
    {
        if (controller.playerApproach)
        {

            if (controller.allKeysIn)
            {
                if (playerDist <= clickDist)
                    GameEvents.ReportInteractHintShown(HintForInteraction.SWITCH);

                else
                    GameEvents.ReportInteractHintShown(HintForInteraction.DISTANCEREQUIRED);
            }

            else
                GameEvents.ReportInteractHintShown(HintForInteraction.REQUIRKEY);
        }
    }

    private void OnMouseExit()
    {
        GameEvents.ReportInteractHintShown(HintForInteraction.DEFAULT);
    }
}
