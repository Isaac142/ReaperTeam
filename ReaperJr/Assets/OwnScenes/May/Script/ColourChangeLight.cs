using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColourChangeLight : MonoBehaviour
{
    //on ignore raycast layer

    public GameObject soul;
    public Light[] lightToControl;

    [Range (0, 5)]
    public int colourIndex = 0;
    private Color[] oriColours;

    private bool canClick = false;

    // Start is called before the first frame update
    void Start()
    {
        soul.SetActive(false);

        oriColours = new Color[lightToControl.Length];

        for(int i = 0; i < lightToControl.Length; i ++)
        {
            oriColours[i] = lightToControl[i].color;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (canClick && !GameManager.Instance.scytheEquiped)
        {
            if (Input.GetMouseButtonDown(0) && lightToControl != null)
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit))
                {
                    Debug.Log(hit.transform.name);
                    if (hit.transform.tag == "Switch" && hit.transform == transform.parent)
                    {
                        colourIndex++;
                    }
                }
            }
        }

        if (colourIndex > 5)
        {
            colourIndex = 0;
        }

        for (int i = 0; i < lightToControl.Length; i++)
        {
            lightToControl[i].color = (colourIndex == 0) ? oriColours[i] : (colourIndex == 1) ? Color.red : (colourIndex == 2) ? Color.green :
                (colourIndex == 3) ? Color.cyan : Color.green;
            lightToControl[i].enabled = (colourIndex == 5) ? false : true;
        }

        if (colourIndex == 4)
        {
            if (soul != null)
                soul.SetActive(true);
        }
        else
        {
            if (soul != null)
                soul.SetActive(false);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            canClick = true;
            if(GetComponentInParent<Renderer>() != null)
                GetComponentInParent<Renderer>().material.EnableKeyword("_EMISSION");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            canClick = false;
            if (GetComponentInParent<Renderer>() != null)
                GetComponentInParent<Renderer>().material.DisableKeyword("_EMISSION");
        }
    }
}