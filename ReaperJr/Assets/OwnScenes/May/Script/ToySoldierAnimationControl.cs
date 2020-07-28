using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToySoldierAnimationControl : ReaperJr
{
    public FakeSoulCollider controlScript;
    public Animator anme;
    
    // Update is called once per frame
    void Update()
    {
        if(controlScript.isWalking)
        {
            anme.SetBool("Walking", true);
        }
        else
            anme.SetBool("Walking", false);
    }
}
