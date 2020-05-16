using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChildProtectorInSink : MonoBehaviour
{
    public SinkControl sinkController;

    //start is called before the first frame update
    void Start()
    {
        sinkController = GetComponentInParent<SinkControl>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
            sinkController.playerIn = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
            sinkController.playerIn = false;
    }
}
