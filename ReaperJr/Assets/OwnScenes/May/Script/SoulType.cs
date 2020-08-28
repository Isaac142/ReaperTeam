using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class SoulType : ReaperJr
{
    public Souls soul;
    public bool isCollected = false;
    [HideInInspector]
    public Sprite soulIcon;
    private enum SoulTypes { BRAVE, HAPPY, SKITTISH}
    private SoulTypes soulTypes;
    public float radius = 3f;
    private Renderer rend;
    public float transparency = 0.2f;
    public List<GameObject> childObj = new List<GameObject>();

    // Start is called before the first frame update
    void Awake()
    {
        soulIcon = soul.icon;

        if (soul.Type == "Brave")
            soulTypes = SoulTypes.BRAVE;
        if (soul.Type == "Happy")
            soulTypes = SoulTypes.HAPPY;
        if (soul.Type == "Skittish")
            soulTypes = SoulTypes.SKITTISH;
        rend = GetComponent<Renderer>();

        switch (soulTypes)
        {
            case SoulTypes.BRAVE:
                break;
            case SoulTypes.HAPPY:
                break;
            case SoulTypes.SKITTISH:
                {
                    rend.material.DOFade(transparency, "_BaseColor", 0.1f);
                    GameObject detectCollider = new GameObject("Collider");
                    detectCollider.transform.parent = transform;
                    detectCollider.transform.position = transform.position;
                    detectCollider.layer = 2;
                    detectCollider.transform.localScale = new Vector3(1f, 1f, 1f);
                    SphereCollider collider = detectCollider.AddComponent<SphereCollider>();
                    Rigidbody rb = gameObject.AddComponent<Rigidbody>();
                    rb.isKinematic = true;
                    collider.isTrigger = true;
                    collider.radius = radius;
                    foreach(GameObject obj in childObj)
                        obj.SetActive(false);
                    break;
                }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            switch (soulTypes)
            {
                case SoulTypes.SKITTISH:
                    {
                        rend.material.DOFade(0.8f, "_BaseColor", 0.5f);
                        foreach (GameObject obj in childObj)
                            obj.SetActive(true);
                        break;
                    }
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.tag == "Player")
        {
            switch (soulTypes)
            {
                case SoulTypes.SKITTISH:
                    {
                        Color color = rend.material.color;
                        color.a = 0.8f;
                        foreach (GameObject obj in childObj)
                            obj.SetActive(true);
                        break;
                    }
            }
        }

    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            switch (soulTypes)
            {
                case SoulTypes.SKITTISH:
                    {
                        rend.material.DOFade(transparency, "_BaseColor", 0.5f);
                        foreach (GameObject obj in childObj)
                            obj.SetActive(false);
                        break;
                    }
            }
        }
    }

    private void OnEnable()
    {
        GameEvents.OnSoulCollected += OnSoulCollected;
    }

    private void OnDisable()
    {
        GameEvents.OnSoulCollected -= OnSoulCollected;
    }

    void OnSoulCollected(SoulType soulType)
    {
        if (soulType == this)
        {
            //isCollected = true;
            //UIManager.INSTANCE.UpdateSouls();
            this.gameObject.SetActive(false);
            GameEvents.ReportCollectHintShown(HintForItemCollect.DEFAULT);
        }      
    }
}
