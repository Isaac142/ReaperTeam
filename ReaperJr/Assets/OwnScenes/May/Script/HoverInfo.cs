using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HoverInfo : MonoBehaviour
{
    private UpdateUI uiScript;
    private GameObject infoPanel;
    public string objectName;
    public string objectType;
    public string description;
    // Start is called before the first frame update
    void Start()
    {
        uiScript = FindObjectOfType<UpdateUI>();
        infoPanel = uiScript.infoPanel;
    }

    private void OnMouseEnter()
    {
        infoPanel.SetActive(true);
        uiScript.itemName.text = ("Object Name: " + objectName);
        uiScript.itemType.text = ("Object Type: " + objectType);
        uiScript.itemDescription.text = ("Description: " + description);
    }

    private void OnMouseExit()
    {
        infoPanel.SetActive(false);
    }
}
