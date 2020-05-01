using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FakeSoulCollider : MonoBehaviour
{
    public List<GameObject> dummies;
    private List<EnemyPatrol> enemyPartrolScripts = new List<EnemyPatrol>();

    // Start is called before the first frame update
    void Start()
    {
        foreach (GameObject dummy in dummies)
            enemyPartrolScripts.Add(dummy.transform.GetComponentInChildren<EnemyPatrol>());

        foreach (EnemyPatrol script in enemyPartrolScripts)
            script.enabled = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            foreach (EnemyPatrol script in enemyPartrolScripts)
                script.enabled = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            foreach (EnemyPatrol script in enemyPartrolScripts)
                script.enabled = false;
        }
    }
}