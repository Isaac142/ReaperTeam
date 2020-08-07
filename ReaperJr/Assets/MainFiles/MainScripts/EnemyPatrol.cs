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
    
    [HideInInspector]
    public float toPlayer;
    private int patrolIndex = 0;

    private enum EnemyType { ENEMY, DUMMY, FLEE, FAKESOUL }
    private EnemyType enemyType;
    public bool isMouse = false, isDog = false, isToySoldier = false, isFakeSoul = false;
    public Animator anim;
    public float fakeSoulTurnTime = 1f;
    bool isChasing = false; //for both dog and mouse, if is chasing, play running animation, otherwise play walking animation, unless there's other animations.
    bool following = false;

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
        if (transform.tag == "Flee")
        {
            enemyType = EnemyType.FLEE;
        }
        if (transform.tag == "FakeSoul")
        {
            enemyType = EnemyType.FAKESOUL;
        }

        player = _PLAYER.gameObject.transform;

        switch (enemyType)
        {
            case EnemyType.DUMMY:
                agent.isStopped = true;
                break;
            case EnemyType.FAKESOUL:
                agent.isStopped = true;
                anim.SetBool("Red", false);
                break;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (_GAME.isPaused)
            return;

        toPlayer = Vector3.Distance(player.position, transform.position);

        if (isDog)
            anim.SetBool("DogChase", isChasing);

        //if is Mouse ==> setbool to same as ischasing, when ischasing true, mouse is running
        if (isMouse)
            anim.SetBool("MouseChase", isChasing);
        switch (enemyType)
        {
            case EnemyType.ENEMY:
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
                bool inSight = Physics.Linecast(transform.position, player.position);
                if (toPlayer >= awareDistance || !inSight || _GAME.gameState == GameState.DEAD)
                {
                    following = false;
                    StartCoroutine(HintOff());
                    agent.SetDestination(patrolPoints[0].position);
                    agent.speed = patrolSpeed;
                    if (agent.remainingDistance <= 0f)
                    {
                        agent.isStopped = true;
                        anim.SetBool("Red", false);
                    }
                }
                else
                    agent.SetDestination(player.transform.position);
                break;
        }
    }

    void NextPatrolPoint()
    {
        if (patrolPoints.Count > 0)
        {
            agent.destination = patrolPoints[patrolIndex].position;
            patrolIndex++;
            patrolIndex %= patrolPoints.Count; //cycling index number.
        }
    }

    public IEnumerator Flee()
    {
        Vector3 runDirection = Vector3.zero;
        if (toPlayer <= awareDistance)
        {
            isChasing = true;

            agent.speed = chasingSpeed;
            runDirection = (transform.position - _PLAYER.gameObject.transform.position);
            Vector3 newPosition = transform.position + runDirection;
            agent.SetDestination(newPosition);
            if (agent.remainingDistance < 0.5f)
                NextPatrolPoint();

            if (isMouse)
                _AUDIO.Play("MouseRunning"); //only play audios when player is around.
        }
        else
        {
            isChasing = false;
            if (isMouse)
                _AUDIO.StopPlay("MouseRunning");
        }

        yield return null;
    }

    public IEnumerator Chasing()
    {
        if (toPlayer < awareDistance && _GAME.gameState == GameState.INGAME)
        {
            if (isMouse)
                _AUDIO.Play("MouseRunning");

            isChasing = true;
            if (Physics.Linecast(transform.position, _PLAYER.transform.position)) //check if character is in sight
            {
                if (!_GAME.isInvincible)
                {
                    if (toPlayer > touchPlayerDist) //preventing enemy pushes character
                    {
                        transform.LookAt(player);
                        agent.destination = player.position;
                        agent.speed = chasingSpeed;
                        isChasing = true;

                        if (isDog)
                        {
                            _AUDIO.Play("DogSnarl");
                            _AUDIO.StopPlay("Rattling");
                        }

                    }
                }
                else
                {
                    if (isDog)
                    {
                        _AUDIO.Play("Rattling");
                        _AUDIO.StopPlay("DogSnarl");
                    }
                    NextPatrolPoint();
                    agent.speed = patrolSpeed;
                    isChasing = false;
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

            if (isMouse)
                _AUDIO.StopPlay("MouseRunning");

        }
        yield return null;
    }

    public IEnumerator FakeSoulActivate()
    {
        GameEvents.ReportCollectHintShown(HintForItemCollect.FAKESOULWARNING);
        anim.SetBool("Red", true);
        yield return new WaitForSeconds(fakeSoulTurnTime);
        agent.isStopped = false;
        following = true;
    }

    IEnumerator HintOff()
    {
        if(!following && !agent.isStopped)
            GameEvents.ReportCollectHintShown(HintForItemCollect.DEFAULT);
        yield return new WaitForSeconds(0.2f);
        StopCoroutine(HintOff());
    }

    private void OnEnable()
    {
        GameEvents.OnFakeSoulChasing += OnFakeSoulChasing;
    }
    private void OnDisable()
    {
        GameEvents.OnFakeSoulChasing -= OnFakeSoulChasing;
    }

    void OnFakeSoulChasing(EnemyPatrol fakesoul)
    {
        if (fakesoul == this)
        {
            StartCoroutine(FakeSoulActivate());
        }
    }
}
