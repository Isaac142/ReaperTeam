using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TouchPlayer : ReaperJr
{
    private EnemyPatrol parentScript;
    public bool isToyGun, isDog, isMouse;

    private void Start()
    {
        parentScript = GetComponentInParent<EnemyPatrol>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (parentScript != null)
        {
            if (other.tag == "Player")
            {
                if (!parentScript.agent.isStopped && _GAME.gameState == GameState.INGAME)
                {
                    if (!_GAME.isInvincible)
                    {
                        GameEvents.ReportGameStateChange(GameState.DEAD);
                        if(isDog || isMouse)
                        _AUDIO.Play("PlayerImpact");
                        if (isToyGun)
                            _AUDIO.Play("ToyGun");
                    }
                }
            }
        }
    }
}
