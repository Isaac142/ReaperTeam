using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckPoint : ReaperJr
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
            _GAME.checkPoints.Add(transform.position);
    }
}
