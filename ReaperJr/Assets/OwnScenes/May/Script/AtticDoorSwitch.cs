using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class AtticDoorSwitch : ReaperJr
{
    public List<KeyItem> keyItems = new List<KeyItem>();

    public Vector3 inRot;
    public Vector3 finalRot;
    public float finalPos = 0;
    [HideInInspector]
    public bool doorLocked = false, playerApproach = false, allKeysIn = false, switchActivated = false;
    public Animator anim;

    // Start is called before the first frame update
    void Start()
    {
        doorLocked = false;
        playerApproach = false;
        allKeysIn = false;
        switchActivated = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (!doorLocked)
            return;

        if (!allKeysIn)
            KeysCheck();
    }

    public void SetKey (KeyItem key, Vector3 keyPos)
    {
        key.transform.position = keyPos;
        key.transform.eulerAngles = inRot;
        key.gameObject.SetActive(true);
    }

    public void FinalPos ()
    {
        foreach(KeyItem key in keyItems)
            key.transform.DOMove(new Vector3(key.transform.position.x, key.transform.position.y, finalPos), 0.5f).OnComplete
                (() => key.transform.DORotate(finalRot, 0.5f));
        doorLocked = false;
    }

    void KeysCheck()
    {
        for (int i = 0; i < keyItems.Count; i++)
        {
            if (!keyItems[i].isInPosition)
                allKeysIn = false;
            else
                allKeysIn = true;
        }
    }
}
