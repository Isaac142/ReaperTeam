using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WonGame : ReaperJr
{
    private ParticleSystem mark;

    // Start is called before the first frame update
    void Start()
    {
        mark = transform.GetChild(0).GetComponent<ParticleSystem>();
        mark.Stop();
    }

    private void Update()
    {
        if (_GAME.totalSoulNo == 0 && _GAME.returnSouls)
            mark.Play();
        else
            mark.Stop();
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            if (_GAME.totalSoulNo == 0 && _GAME.returnSouls)
                GameEvents.ReportGameStateChange(GameState.WON);
        }
    }
}
