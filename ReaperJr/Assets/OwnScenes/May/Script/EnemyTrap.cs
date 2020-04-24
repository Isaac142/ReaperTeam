using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyTrap : MonoBehaviour
{
    public GameObject soul;
    public NavMeshAgent enemy;
    public GameObject lure;
    public Transform enemyGoal;
    public Animator trapDoor;
    public string lureName;
    public float activeTime = 3f;
    private bool lureIn = false;


    // Start is called before the first frame update
    void Start()
    {
        lureIn = false;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.name == lureName)
            lureIn = true;      
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.tag == "Enemy" && lureIn)
        {
            if (Vector3.Distance(other.transform.position, enemyGoal.transform.position) <= 3f)
            {
                trapDoor.SetTrigger("Close");
                if (soul != null)
                {
                    soul.gameObject.tag = "Soul";
                    soul.transform.parent = null;
                }
                enemy.transform.GetChild(0).gameObject.SetActive(false);
                enemy.isStopped = true;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.tag == "Player")
        {
            if(lureIn)
                StartCoroutine("GoalSet");
        }
    }

    IEnumerator GoalSet()
    {
        yield return new WaitForSeconds(activeTime);
        enemy.SetDestination(enemyGoal.position);
    }
}
