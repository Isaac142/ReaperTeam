using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

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
            script.transform.GetComponent<NavMeshAgent>().isStopped = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            isWalking = true;
            foreach (EnemyPatrol script in enemyPartrolScripts)
            {
                script.agent.isStopped = false;

                if (script.isToySoldier)
                {
                    script.anim.SetBool("Walking", true);
                   // _AUDIO.Play("ToyMove");
                }
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
                if (script.isToySoldier)
                {
                    script.anim.SetBool("Walking", false);
                    //_AUDIO.StopPlay("ToyMove");
                }
                script.agent.isStopped = true;
            }
        }
    }
}