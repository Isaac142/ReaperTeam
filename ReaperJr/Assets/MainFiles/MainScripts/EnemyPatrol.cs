using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyPatrol : ReaperJr
{
    public Transform player;
    private NavMeshAgent agent;

    public float awareDistance = 10f;
    public float detectRange = 5f;
    public List<Transform> patrolPoints;
    public float patrolSpeed = 3f;
    public float chasingSpeed = 3f;
    public float touchPlayerDist = 1f;

    private float toPlayer;
    private int patrolIndex = 0;
    
    private enum EnemyType { ENEMY, DUMMY, FLEE}
    private EnemyType enemyType;

    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        if (transform.tag == "Enemy")
        {
            enemyType = EnemyType.ENEMY;
        }
        if (transform.tag == "Dummy")
        {
            enemyType = EnemyType.DUMMY;
        }
        if(transform.tag == "Flee")

        player = _PLAYER.gameObject.transform;
    }

    // Update is called once per frame
    void Update()
    {
        if (_GAME.isPaused)
            return;

        toPlayer = Vector3.Distance(player.position, transform.position);
        switch(enemyType)
        {
            case EnemyType.ENEMY:
                if (toPlayer < awareDistance)
                {
                    transform.LookAt(player);

                    RaycastHit hit;
                    if (Physics.Raycast(transform.position, transform.forward, out hit)) //check if character is in sight
                    {
                        if (hit.transform.tag == "Player")
                        {
                            if (toPlayer > touchPlayerDist) //preventing enemy pushes character
                            {
                                agent.destination = player.position;
                                agent.speed = chasingSpeed;
                            }
                            else
                            {
                                NextPatrolPoint();
                                agent.speed = patrolSpeed;
                            }
                        }
                    }
                }

                if (agent.remainingDistance < 0.5f)
                    NextPatrolPoint();
                break;

            case EnemyType.DUMMY:
                if (agent.remainingDistance < 0.5f)
                    NextPatrolPoint();
                break;

            case EnemyType.FLEE:
                Vector3 runDirection = Vector3.zero;
                if (toPlayer <= detectRange)
                {
                    runDirection = (transform.position - _PLAYER.gameObject.transform.position);
                    Vector3 newPosition = transform.position + runDirection;
                    agent.SetDestination(newPosition);
                    if (agent.remainingDistance < 0.5f)
                        NextPatrolPoint();
                }
                break;
        }            
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
