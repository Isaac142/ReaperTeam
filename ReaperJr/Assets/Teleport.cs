using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Teleport : MonoBehaviour
{
    public Transform target;
    public GameObject player;

 
    void OnTriggerEnter(Collider col)
    {
        player = GameObject.Find("Player");

        if (col.gameObject.tag == "teleport")
            player.transform.position = target.position;
    }
}
