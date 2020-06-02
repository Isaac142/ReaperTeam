using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchControl : ReaperJr
{
    //on ignore raycast layer

    public GameObject pLight, soul;
    private bool canClick = false, lightsOn = false;

    // Start is called before the first frame update
    void Start()
    {
        pLight.SetActive(false);
        soul.SetActive(false);       
    }

    // Update is called once per frame
    void Update()
    {
        if(canClick && !_GAME.scytheEquiped)
        {
            if (Input.GetMouseButtonDown(0))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit))
                {
                    if (hit.transform.tag == "Switch")
                    {
                        lightsOn = !lightsOn;
                    }
                }
            }
        }

        if(lightsOn)
        {
            pLight.SetActive(true);
            if(soul != null)
                soul.SetActive(true);
        }
        else
        {
            pLight.SetActive(false);
            if(soul != null)
            soul.SetActive(false);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
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
