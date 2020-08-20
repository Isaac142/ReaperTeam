using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamagePlayer : ReaperJr
{
    private void OnTriggerEnter(Collider other)
    {
        if (_GAME.gameState == GameState.INGAME)
        {
            if (other.tag == "Player" && !_GAME.isInvincible)
            {
                StartCoroutine(_PLAYER.DeathTimer());
                //GameEvents.ReportGameStateChange(GameState.DEAD);
                _AUDIO.Play(_PLAYER.GetComponent<AudioSource>(), "PlayerImpact", 10);
            }
        }
    }
}
