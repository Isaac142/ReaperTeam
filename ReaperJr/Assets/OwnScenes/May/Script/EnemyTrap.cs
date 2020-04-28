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
    public float activeTime = 3f;
    private bool lureIn = false;
    private bool catched = false;

    // Start is called before the first frame update
    void Start()
    {
        lureIn = false;
    }
    private void Update()
    {
        if (catched)
        {
            lure.GetComponent<ItemMovement>().canHold = false;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Lure")
            lureIn = true;
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.tag == "Enemy")
        {
            if (lureIn)
            { if (Vector3.Distance(other.transform.position, enemyGoal.transform.position) <= 3f)
                {
                    trapDoor.SetTrigger("Close");
                    if (soul != null)
                    {
                        soul.gameObject.tag = "Soul";
                        soul.transform.parent = null;
                    }
                    catched = true;
                }
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
        if (other.tag == "Lure")
            lureIn = false;
    }

    IEnumerator GoalSet()
    {
        yield return new WaitForSeconds(activeTime);
        enemy.SetDestination(enemyGoal.transform.position);
    }
}
