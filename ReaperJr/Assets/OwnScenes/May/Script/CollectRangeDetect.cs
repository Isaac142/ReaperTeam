using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectRangeDetect : ReaperJr
{
    // Start is called before the first frame update
    void Start()
    {
        if (GetComponent<SphereCollider>() == null)
            this.gameObject.AddComponent<SphereCollider>();
        GetComponent<SphereCollider>().radius = (_PLAYER.collectableDist + 1f)/this.transform.parent.transform.localScale.x;
        GetComponent<SphereCollider>().isTrigger = true;
        this.gameObject.layer = 2; // set at ignore raycast layer
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player" && !_GAME.isHolding)
        {
            _PLAYER.canCollect = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player" && !_GAME.isHolding)
        {
            _PLAYER.canCollect = false;
            GameEvents.ReportCollectHintShown(HintForItemCollect.DEFAULT);
        }
    }
}
