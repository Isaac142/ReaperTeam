using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TouchPlayer : ReaperJr
{
    private EnemyPatrol parentScript;

    private void Start()
    {
        parentScript = GetComponentInParent<EnemyPatrol>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (parentScript != null && _GAME.gameState == GameState.INGAME)
        {
            if (parentScript.enabled == true)
            {
                if (other.tag == "Player")
                    GameEvents.ReportGameStateChange(GameState.DEAD);
            }
        }
    }
}
