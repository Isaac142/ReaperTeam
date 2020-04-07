using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyPatrol : MonoBehaviour
{
    public Transform player;
    private NavMeshAgent agent;

    public float awareDistance = 10f;
    public List<Transform> patrolPoints;

    private float toPlayer;
    private int patrolIndex = 0;
    

    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();

    }

    // Update is called once per frame
    void Update()
    {
        toPlayer = Vector3.Distance(player.position, transform.position);

        if(toPlayer < awareDistance)
        {
            transform.LookAt(player);

            RaycastHit hit;
            if(Physics.Raycast(transform.position, transform.forward, out hit )) //check if character is in sight
            {
                if(hit.transform.tag == "Player")
                {
                    if (toPlayer > 2f) //preventing enemy pushes character
                    {
                        agent.destination = player.position;
                    }
                    else
                        NextPatrolPoint();
                }
            }
        }

        if (agent.remainingDistance < 0.5f)
            NextPatrolPoint();       
    }

    void NextPatrolPoint()
    {
        if(patrolPoints.Count > 0)
        {
            agent.destination = patrolPoints[patrolIndex].position;
            patrolIndex++;
            patrolIndex %= patrolPoints.Count; //cycling index number.
        }
    }

}
