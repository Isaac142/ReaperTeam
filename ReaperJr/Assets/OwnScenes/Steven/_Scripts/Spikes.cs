using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spikes : MonoBehaviour
{
    void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            Application.LoadLevel(Application.loadedLevel);
            Debug.Log(" You Died !");
        }
    }
}
