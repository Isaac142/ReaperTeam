using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoulType : MonoBehaviour
{
    public Souls soul;
    public Sprite soulIcon;
    public Sprite soulMask;
    private enum SoulTypes { BRAVE, HAPPY, SKITTISH}
    private SoulTypes soulTypes;
    public float radius = 3f;
    private Color color;

    // Start is called before the first frame update
    void Awake()
    {
        soulIcon = soul.icon;
        soulMask = soul.mask;

        if (soul.Type == "Brave")
            soulTypes = SoulTypes.BRAVE;
        if (soul.Type == "Happy")
            soulTypes = SoulTypes.HAPPY;
        if (soul.Type == "Skittish")
            soulTypes = SoulTypes.SKITTISH;
        color = GetComponent<Renderer>().material.color;

        switch(soulTypes)
        {
            case SoulTypes.BRAVE:
                {
                    GetComponent<Renderer>().material.SetColor("_Color", new Color(color.r, color.b, color.g, color.a));
                    transform.GetChild(0).gameObject.SetActive(true);
                    break;
                }
            case SoulTypes.HAPPY:
                {
                    GetComponent<Renderer>().material.SetColor("_Color", new Color(color.r, color.b, color.g, color.a));
                    transform.GetChild(0).gameObject.SetActive(true);
                    break;
                }
            case SoulTypes.SKITTISH:
                {
                    GetComponent<Renderer>().material.SetColor("_Color", new Color(color.r, color.b, color.g, 0f));
                    transform.GetChild(0).gameObject.SetActive(false);
                    SphereCollider collider = gameObject.AddComponent<SphereCollider>();
                    collider.isTrigger = true;
                    collider.radius = radius;
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
                        GetComponent<Renderer>().material.SetColor("_Color", new Color(color.r, color.b, color.g, color.a));
                        transform.GetChild(0).gameObject.SetActive(false);
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
                        GetComponent<Renderer>().material.SetColor("_Color", new Color(color.r, color.b, color.g, 0f));
                        transform.GetChild(0).gameObject.SetActive(false);
                        break;
                    }
            }
        }
    }
}
