using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Restart : MonoBehaviour
{
    

    // Update is called once per frame
    void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            Application.LoadLevel(Application.loadedLevel);
            Debug.Log(" Level restarted !! ");
        }
    }
}
