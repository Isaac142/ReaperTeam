using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FakeSoulCollider : ReaperJr
{
    public List<GameObject> dummies;
    private List<EnemyPatrol> enemyPartrolScripts = new List<EnemyPatrol>();
    public bool isWalking = false;

    // Start is called before the first frame update
    void Start()
    {
        foreach (GameObject dummy in dummies)
            enemyPartrolScripts.Add(dummy.transform.GetComponent<EnemyPatrol>());
        foreach (EnemyPatrol script in enemyPartrolScripts)
            script.agent.isStopped = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            isWalking = true;
            foreach (EnemyPatrol script in enemyPartrolScripts)
            {
                if (script != null)
                    script.agent.isStopped = false;
            }
        }
    }


    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            isWalking = false;
            foreach (EnemyPatrol script in enemyPartrolScripts)
            {
                if (script != null)
                    script.agent.isStopped = true;
            }
        }
    }
}