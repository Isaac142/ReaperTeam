using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyFlee : MonoBehaviour
{
    public GameObject player;

    private NavMeshAgent agent;
    public float detectRange = 5f;
    public List<Transform> fleePoint;
    public int fleePointIndex;

    private Vector3 runDirection;
    bool RunDirTest(Vector3 newPos, out Vector3 result)
    {
        NavMeshHit hit;
        if (NavMesh.SamplePosition(newPos, out hit, 1.0f, NavMesh.AllAreas))
        {
            result = hit.position;
            return true;
        }
        result = Vector3.zero;
        return false;
    }
    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    // Update is called once per frame
    void Update()
    {
        float distance = Vector3.Distance(player.transform.position, transform.position);
        if(distance <= detectRange)
        {
            runDirection = (transform.position - player.transform.position);
            Vector3 newPosition = transform.position + runDirection;
            Vector3 newPos;

            if (RunDirTest(newPosition, out newPos))
                agent.SetDestination(newPosition);
            else
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
