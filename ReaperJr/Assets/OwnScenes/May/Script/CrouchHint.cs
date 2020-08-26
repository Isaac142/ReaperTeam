using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrouchHint : ReaperJr
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.tag == "Player")
            GameEvents.ReportInteractHintShown(HintForInteraction.CROUCH);
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.transform.tag == "Player")
            GameEvents.ReportInteractHintShown(HintForInteraction.DEFAULT);
    }
}
