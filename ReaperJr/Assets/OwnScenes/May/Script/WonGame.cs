using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WonGame : MonoBehaviour
{
    private ParticleSystem mark;

    // Start is called before the first frame update
    void Start()
    {
        mark = transform.GetChild(0).GetComponent<ParticleSystem>();
        mark.Stop();
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            if (GameManager.Instance.totalSoulNo == 0)
                GameManager.Instance.wonGame = true;
        }
    }
}
