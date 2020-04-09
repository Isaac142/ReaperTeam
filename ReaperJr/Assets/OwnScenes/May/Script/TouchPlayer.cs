using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TouchPlayer : MonoBehaviour
{
    private EnemyPatrol parentScript;

    private void Start()
    {
        parentScript = GetComponentInParent<EnemyPatrol>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (parentScript.enabled == true)
        {
            if (other.tag == "Player")
                GameManager.Instance.dead = true;
        }
    }
}
