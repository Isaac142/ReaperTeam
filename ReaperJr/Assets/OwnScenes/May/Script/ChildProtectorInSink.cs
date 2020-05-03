using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChildProtectorInSink : MonoBehaviour
{
    public SinkControl sinkController;

    // Start is called before the first frame update
    void Start()
    {
        sinkController = GetComponentInParent<SinkControl>();
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            if (sinkController.player == null)
                sinkController.player = other.gameObject;

            if (!sinkController.fillWater)
                sinkController.filmOn = true;

            sinkController.playerIn = true;
        }
    }

        private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            sinkController.playerIn = false;
            sinkController.filmOn = false;
        }
    }
}
