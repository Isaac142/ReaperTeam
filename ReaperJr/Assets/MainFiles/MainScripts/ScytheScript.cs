using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScytheScript : ReaperJr
{
    public bool activated;

    private void OnCollisionEnter(Collision collision)
    {
        activated = false;
        GetComponent<Rigidbody>().isKinematic = true;
        _PLAYER.teleportEnd.SetActive(true);
    }
}
