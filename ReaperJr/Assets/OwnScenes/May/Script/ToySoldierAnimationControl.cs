using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToySoldierAnimationControl : ReaperJr
{
    public FakeSoulCollider controlScript;
    public Animator anim;

    private void Start()
    {
        anim = GetComponent<Animator>();
        if (anim == null)
            this.enabled = false;
    }

    // Update is called once per frame

    void Update()
    {
        if(controlScript.isWalking)
        {
            anim.SetBool("Walking", true);
        }
        else
            anim.SetBool("Walking", false);
    }
}
