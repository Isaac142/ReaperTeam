using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FakeSoulController : ReaperJr
{
    public bool instantDeath = false;
    public Animator anim;

    private void Update()
    {
        if (_GAME.gameState == GameState.DEAD)
        {
            GameEvents.ReportCollectHintShown(HintForItemCollect.DEFAULT);
            if (instantDeath)
                anim.SetBool("Red", false);
            else
                anim.SetBool("ScaleUp", false);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            if (instantDeath)
                anim.SetBool("Red", true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            if (instantDeath)
                anim.SetBool("Red", false);
            else
            {
                GameEvents.ReportCollectHintShown(HintForItemCollect.DEFAULT);
                anim.SetBool("ScaleUp", false);
            }
        }
    }

    private void OnEnable()
    {
        GameEvents.OnFakeSoulCollected += OnFakeSoulScaleCollected;
    }
    private void OnDisable()
    {
        GameEvents.OnFakeSoulCollected -= OnFakeSoulScaleCollected;
    }

    void OnFakeSoulScaleCollected(FakeSoulController fakesoul)
    {
        if (fakesoul == this)
        {
            if (!instantDeath)
            {
                GameEvents.ReportCollectHintShown(HintForItemCollect.FAKESOULWARNING);
                anim.SetBool("ScaleUp", true);
            }
        }
    }
}
