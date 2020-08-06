using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyActivator : ReaperJr
{
    public List<EnemyPatrol> enemies = new List<EnemyPatrol>();

    private void OnTriggerStay(Collider other)
    {
        if(other.tag == "Player")
        {
            foreach (EnemyPatrol enemy in enemies)
                StartCoroutine(enemy.Chasing());
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            foreach (EnemyPatrol enemy in enemies)
                StopCoroutine(enemy.Chasing());
        }
    }
}
