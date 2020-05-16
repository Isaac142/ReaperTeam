using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bubble : MonoBehaviour
{
    public float delayTime = 2f;

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
            StartCoroutine("Disappear");
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            StopCoroutine("Disappear");
            transform.GetChild(0).gameObject.SetActive(true);
        }
    }


    IEnumerator Disappear()
    {
        yield return new WaitForSeconds(delayTime);
        transform.GetChild(0).gameObject.SetActive(false);
    }
}
