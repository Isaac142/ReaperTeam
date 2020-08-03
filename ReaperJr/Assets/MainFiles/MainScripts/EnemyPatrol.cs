using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyPatrol : ReaperJr
{

    public Transform player;
    [HideInInspector]
    public NavMeshAgent agent;

    public float awareDistance = 10f;
    public List<Transform> patrolPoints;
    public float patrolSpeed = 3f;
    public float chasingSpeed = 3f;
    public float touchPlayerDist = 1f;

    private float toPlayer;
    private int patrolIndex = 0;
    
    private enum EnemyType { ENEMY, DUMMY, FLEE, FAKESOUL}
    private EnemyType enemyType;
    public bool isMouse = false, isDog = false, isToySoldier = false, isFakeSoul = false;
    public Animator anim;

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
        {
            enemyType = EnemyType.FLEE;
        }
        if (transform.tag == "FakeSoul")
        {
            enemyType = EnemyType.FAKESOUL;
        }

        player = _PLAYER.gameObject.transform;

        switch(enemyType)
        {
            case EnemyType.DUMMY:
                agent.isStopped = true;
                break;
            case EnemyType.FAKESOUL:
               // agent.isStopped = true;
                break;
        }
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
                StartCoroutine(Chasing());
                if (agent.remainingDistance < 0.5f)
                    NextPatrolPoint();
                break;

            case EnemyType.DUMMY:
                
                if (agent.remainingDistance < 0.5f)
                    NextPatrolPoint();
                break;

            case EnemyType.FLEE:
                StartCoroutine(Flee());
                if (agent.remainingDistance < 0.5f)
                {
                    NextPatrolPoint();
                    agent.speed = patrolSpeed;
                }
                break;

            case EnemyType.FAKESOUL:
                Chasing();
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
            isDogChasing = false;
            isDogWalking = true;
            _AUDIO.Play("Rattling");
            anim.SetBool("DogWalk", isDogWalking);
        }
    }

    public IEnumerator Flee()
    {
        Vector3 runDirection = Vector3.zero;
        if (toPlayer <= awareDistance)
        {
            if (isMouse)
            {
                _AUDIO.Play("MouseRunning");
            }

            agent.speed = chasingSpeed;
            runDirection = (transform.position - _PLAYER.gameObject.transform.position);
            Vector3 newPosition = transform.position + runDirection;
            agent.SetDestination(newPosition);
            if (agent.remainingDistance < 0.5f)
                NextPatrolPoint();
        } 
        else
            _AUDIO.StopPlay("MouseRunning");

        yield return null;
    }


    bool isDogChasing;
    bool isDogWalking;
    public IEnumerator Chasing()
    {
        if (toPlayer < awareDistance && _GAME.gameState == GameState.INGAME)
        {
            if (isDog)
            {
                isDogChasing = true;
                isDogWalking = false;
                _AUDIO.Play("Rattling");
                anim.SetBool("DogChase", isDogChasing);
            }

            if (isMouse)
            {
                _AUDIO.Play("MouseRunning");
            }
            if (!_GAME.isInvincible)
            {
                transform.LookAt(player);

                RaycastHit hit;
                if (Physics.Raycast(transform.position, transform.forward, out hit)) //check if character is in sight
                {
                    if (hit.transform.tag == "Player")
                    {
                        if (toPlayer > touchPlayerDist) //preventing enemy pushes character
                        {
                            if(isDog)
                            {
                                _AUDIO.StopPlay("Rattling");
                                _AUDIO.Play("DogSnarl");
                            }
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
        }

        else
        {
            if (isDog)
            {
                _AUDIO.StopPlay("DogSnarl");
                _AUDIO.StopPlay("Rattling");
            }

            if(isMouse)
            {
                _AUDIO.StopPlay("MouseRunning");
            }

        }
        yield return null;
    }

    public IEnumerator FakeSoulActivate()
    {
        GameEvents.ReportCollectHintShown(HintForItemCollect.FAKESOULWARNING);
        yield return new WaitForSeconds(3f);
        agent.isStopped = false;
    }

    //private void OnEnable()
    //{
    //    GameEvents.OnFakeSoulChasing += OnFakeSoulChasing;
    //}
    //private void OnDisable()
    //{
    //    GameEvents.OnFakeSoulChasing -= OnFakeSoulChasing;
    //}

    //void OnFakeSoulChasing(EnemyPatrol fakesoul)
    //{
    //    if(fakesoul == this)
    //    StartCoroutine(FakeSoulActivate());
    //    Chasing();
    //}
}
