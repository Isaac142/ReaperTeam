using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyFlee : ReaperJr
{
    public GameObject player;

    private NavMeshAgent agent;
    public float detectRange = 5f;
    public List<Transform> fleePoint;
    public int fleePointIndex;

    private Vector3 runDirection;

    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    // Update is called once per frame
    void Update()
    {
        if (_GAME.isPaused)
            return;

        float distance = Vector3.Distance(_PLAYER.gameObject.transform.position, transform.position);
        if(distance <= detectRange)
        {
            runDirection = (transform.position - _PLAYER.gameObject.transform.position);
            Vector3 newPosition = transform.position + runDirection;
            agent.SetDestination(newPosition);
            if (agent.remainingDistance < 0.5f)
                NextFleePoint();
        }
    }

    void NextFleePoint()
    {
        if (fleePoint.Count > 0)
        {
            agent.destination = fleePoint[fleePointIndex].position;
            fleePointIndex++;
            fleePointIndex %= fleePoint.Count;
        }
    }
}
