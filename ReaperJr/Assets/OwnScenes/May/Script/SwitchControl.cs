using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchControl : ReaperJr
{
    //on ignore raycast layer

    public List<GameObject> pLights = new List<GameObject>();
    public List<SoulType> souls = new List<SoulType>();
    private bool canClick = false, lightsOn = false;
    private float playerDist;
    public float clickDist;
    public Animator anim;

    // Start is called before the first frame update
    void Start()
    {
        foreach (GameObject light in pLights)
            light.SetActive(false);
        foreach (SoulType soul in souls)
            soul.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        playerDist = Vector3.Distance(_PLAYER.transform.position, this.transform.position);

        if (canClick)
        {
            if (Input.GetMouseButtonDown(1))
            {
                if (playerDist <= clickDist)
                {
                    lightsOn = !lightsOn;
                }
            }
        }

        if (lightsOn)
        {
            anim.SetBool("SwitchOn", true);
            foreach (GameObject light in pLights)
                light.SetActive(true);
            if (souls.Count > 0)
            {
                foreach (SoulType soul in souls)
                {
                    if (!soul.isCollected)
                        soul.gameObject.SetActive(true);
                }
            }
        }
        else
        {
            anim.SetBool("SwitchOn", false);
            foreach (GameObject light in pLights)
                light.SetActive(false);
            if (souls.Count > 0)
            {
                foreach (SoulType soul in souls)
                    soul.gameObject.SetActive(false);
            }
        }
    }

    private void OnMouseOver()
    {
        if (canClick)
        {
            if (playerDist <= clickDist)
                GameEvents.ReportInteractHintShown(HintForInteraction.SWITCH);

            else
                GameEvents.ReportInteractHintShown(HintForInteraction.DISTANCEREQUIRED);
        }
    }

    private void OnMouseExit()
    {
        GameEvents.ReportInteractHintShown(HintForInteraction.DEFAULT);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            canClick = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            canClick = false;
        }
    }
}
