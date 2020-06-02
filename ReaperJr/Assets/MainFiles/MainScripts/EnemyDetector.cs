using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDetector : ReaperJr
{
    //on ignore raycast layer

    public GameObject soul;

    private void Start()
    {
        soul.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag =="Enemy" && soul != null)
        {
            soul.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Enemy" && soul != null)
        {
            soul.SetActive(false);
        }
    }
}
