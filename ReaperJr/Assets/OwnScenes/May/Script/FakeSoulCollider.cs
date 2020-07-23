using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FakeSoulCollider : ReaperJr
{
    public List<GameObject> dummies;
    private List<EnemyPatrol> enemyPartrolScripts = new List<EnemyPatrol>();

    // Start is called before the first frame update
    void Start()
    {
        foreach (GameObject dummy in dummies)
        {
            enemyPartrolScripts.Add(dummy.transform.GetComponent<EnemyPatrol>());
        }

        foreach (EnemyPatrol script in enemyPartrolScripts)
            script.enabled = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            foreach (EnemyPatrol script in enemyPartrolScripts)
            {
                if(script != null)
                script.enabled = true;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            foreach (EnemyPatrol script in enemyPartrolScripts)
            {
                if (script != null)
                    script.enabled = false;
            }
        }
    }
}