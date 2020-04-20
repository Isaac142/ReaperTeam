using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeigthSwitchController : MonoBehaviour
{
    public Animator controlledObject;
    public string entreAction, exitAction;

    private void OnTriggerEnter(Collider other)
    {
        controlledObject.SetTrigger(entreAction);
        controlledObject.ResetTrigger(exitAction);
    }

    private void OnTriggerExit(Collider other)
    {
        controlledObject.SetTrigger(exitAction);
        controlledObject.ResetTrigger(entreAction);
    }
}
