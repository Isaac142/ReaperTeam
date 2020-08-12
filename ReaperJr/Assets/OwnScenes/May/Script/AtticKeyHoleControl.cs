using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AtticKeyHoleControl : ReaperJr
{
    private AtticDoorSwitch controller;
    public int keyIndex;
    public float clickDist = 1.5f;
    private float playerDist;
    public Vector3 keyPos;

    private void Start()
    {
        controller = GetComponentInParent<AtticDoorSwitch>();
        this.gameObject.layer = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (!controller.doorLocked)
            return;

        if (controller.playerApproach && controller.keyItems[keyIndex].isCollected)
        {
            playerDist = Vector3.Distance(_PLAYER.transform.position, this.transform.position);
        }

        if (controller.playerApproach)
        {

            if (playerDist <= clickDist && controller.keyItems[keyIndex].isCollected && !controller.keyItems[keyIndex].isInPosition)
            {
                if (Input.GetMouseButtonDown(1))
                {
                    controller.SetKey(controller.keyItems[keyIndex], keyPos);
                }
            }
        }
    }

    private void OnMouseOver()
    {
        if (controller.playerApproach)
        {
            if (controller.keyItems[keyIndex].isCollected)
            {
                if (!controller.keyItems[keyIndex].isInPosition)
                {
                    if (playerDist <= clickDist)
                        GameEvents.ReportInteractHintShown(HintForInteraction.SWITCH);
                    else
                        GameEvents.ReportInteractHintShown(HintForInteraction.DISTANCEREQUIRED);
                }
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
