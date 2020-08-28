﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoulJar : ReaperJr
{
    public GameObject lid;
    Vector3 returnPos = Vector3.zero;
    Vector3 returnRot = Vector3.zero;
    Vector3 lidPos = Vector3.zero;
    Vector3 lidRot = Vector3.zero;
    private bool playerIn = false;

    // Start is called before the first frame update
    void Start()
    {
        returnPos = new Vector3(-6.064735f, -1.204609f, 3.368043f);
        returnRot = Vector3.zero;
        lidPos = new Vector3(0, 5.6f, 0);
        lidRot = Vector3.zero;        
    }

    private void OnMouseOver()
    {
        GetComponent<Renderer>().material.EnableKeyword("_EMISSION");
        if (!_GAME.returnSouls)
        {
            if (_GAME.totalSoulNo == 0)
            {
                if (playerIn)
                {
                    GameEvents.ReportInteractHintShown(HintForInteraction.RETURN);

                    if (Input.GetMouseButtonDown(1))
                    {
                        this.transform.localPosition = returnPos;
                        this.transform.localEulerAngles = returnRot;
                        lid.transform.localPosition = lidPos;
                        lid.transform.localEulerAngles = lidRot;
                        GameEvents.ReportInteractHintShown(HintForInteraction.DEFAULT);
                        _GAME.returnSouls = true;
                    }
                }
                else
                    GameEvents.ReportInteractHintShown(HintForInteraction.DISTANCEREQUIRED);
            }
            else
                GameEvents.ReportInteractHintShown(HintForInteraction.JAR);
        }
        else
            return;
    }
    private void OnMouseExit()
    {
        if(!playerIn)
             GetComponent<Renderer>().material.DisableKeyword("_EMISSION");
        if (!_GAME.returnSouls)
            GameEvents.ReportInteractHintShown(HintForInteraction.DEFAULT);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.tag == "Player")
        {
            playerIn = true;
            GetComponent<Renderer>().material.EnableKeyword("_EMISSION");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.transform.tag == "Player")
        {
            playerIn = false;
            GetComponent<Renderer>().material.DisableKeyword("_EMISSION");
        }
    }
}
