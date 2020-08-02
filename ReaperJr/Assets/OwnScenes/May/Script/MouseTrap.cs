using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using DG.Tweening;

public class MouseTrap : MonoBehaviour
{
    public GameObject soul;
    public NavMeshAgent mouse;
    public ItemMovement lure;
    public Transform mouseGoal;
    public Animator anim;
    public float trapSetTime = 3f;
    private bool lureIn = false;
    private bool catched = false;
    private bool playerIn = false;
    public Transform lurePos;
    public Transform soulPos;

    // Start is called before the first frame update
    void Start()
    {
        lureIn = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Lure")
        {
            if (lure.transform.parent == null)
            {
                lure.transform.position = lurePos.position;
                lure.transform.eulerAngles = Vector3.zero;
                lureIn = true;
                anim.SetBool("TrapOn", true);
                lure.enabled = false;
            }
        }
        if (other.tag == "Player")
        {
            playerIn = true;
            if (!lureIn)
                GameEvents.ReportInteractHintShown(HintForInteraction.REQUIRKEY);
            GetComponentInChildren<Renderer>().sharedMaterial.EnableKeyword("_EMISSION");
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (!playerIn && lureIn)
        {
            StartCoroutine(GoalSet());

            if (other.tag == "Flee")
            {
                if (Vector3.Distance(other.transform.position, mouseGoal.transform.position) < 0.5f)
                {
                    StartCoroutine(TrapSetOff());
                    mouse.isStopped = true;
                    mouse.GetComponent<Rigidbody>().isKinematic = true;
                }

                if (soul != null && catched)
                {
                    soul.gameObject.tag = "Soul";
                    soul.transform.parent = null;
                    soul.transform.DOMove(soulPos.position, 0.5f);
                }
            }
        }

        else return;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            playerIn = false;
            GameEvents.ReportInteractHintShown(HintForInteraction.DEFAULT);
            GetComponentInChildren<Renderer>().sharedMaterial.DisableKeyword("_EMISSION");
        }

        if (other.tag == "Lure")
            lureIn = false;
    }

    IEnumerator GoalSet()
    {
        yield return new WaitForSeconds(trapSetTime);
        StopCoroutine(mouse.GetComponent<EnemyPatrol>().Flee());
        mouse.SetDestination(mouseGoal.transform.position);
    }

    IEnumerator TrapSetOff()
    {
        yield return new WaitForSeconds(1f);
        lure.gameObject.SetActive(false);
        anim.SetBool("TrapOn", false);
        catched = true;

    }
}
