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
    private bool playerIn = false;

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

        if (lureIn && !playerIn)
            StartCoroutine("GoalSet");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Lure")
            lureIn = true;
        if (other.tag == "Player")
            playerIn = true;
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.tag == "Enemy")
        {
            if (lureIn && !playerIn)
            {
                if (Vector3.Distance(other.transform.position, enemyGoal.transform.position) < 1f)
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
            playerIn = false;

        if (other.tag == "Lure")
            lureIn = false;
    }

    IEnumerator GoalSet()
    {
        yield return new WaitForSeconds(activeTime);
        enemy.SetDestination(enemyGoal.transform.position);
    }
}
