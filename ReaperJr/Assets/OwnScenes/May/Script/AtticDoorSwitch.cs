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

    private void Update()
    {
        for(int i = 0; i < keyItems.Count; i ++) // preventing holding more than one key at anytime
        {
            if (keyItems[i].isCollected && !keyItems[i].isInPosition)
                _PLAYER.keyCollect = false;
            else
                _PLAYER.keyCollect = true;
        }
    }

    public void SetKey(KeyItem key, Vector3 keyPos)
    {
        key.transform.position = keyPos;
        key.transform.eulerAngles = inRot;
        key.gameObject.SetActive(true);
        key.isInPosition = true;
        KeysCheck();
    }

    public void FinalPos()
    {
        foreach (KeyItem key in keyItems)
            key.transform.DOMove(new Vector3(key.transform.position.x, key.transform.position.y, finalPos), 0.5f).OnComplete
                (() => key.transform.DORotate(finalRot, 0.5f));
        doorLocked = false;
    }

    void KeysCheck() // check if all key has been collected
    {
        List<KeyItem> temp = keyItems.FindAll(x => x.isInPosition == true);
        if (temp.Count == keyItems.Count)
            allKeysIn = true;
    }
}
