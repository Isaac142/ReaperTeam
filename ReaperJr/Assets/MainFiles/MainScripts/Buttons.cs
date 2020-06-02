using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Buttons : ReaperJr
{
    public UpdateUI uiScript;

    private void Awake()
    { 
        if(uiScript == null)
            uiScript = FindObjectOfType<Canvas>().GetComponent<UpdateUI>();
    }

    private void OnMouseOver()
    {
        _GAME.playerActive = false;
    }

    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        _GAME.Restart();
    }

    public void Return()
    {
        _GAME.pausePanel = false;
        _GAME.isPaused = false;
        _GAME.menuPanel = false;
    }

    public void Menu()
    {
        _GAME.menuPanel = true;
        uiScript.instrunctionPanel.SetActive(true);
        uiScript.controlsPanel.SetActive(true);
        uiScript.uiPanel.SetActive(false);
        uiScript.optionPanel.SetActive(false);
    }

    public void ControlsPanel()
    {
        uiScript.controlsPanel.SetActive(true);
        uiScript.uiPanel.SetActive(false);
    }
    
    public void UIsPanel()
    {
        uiScript.controlsPanel.SetActive(false);
        uiScript.uiPanel.SetActive(true);
    }

    public void OptionPanel()
    {
        uiScript.instrunctionPanel.SetActive(false);
        uiScript.optionPanel.SetActive(true);
    }

    public void InstructionPanel()
    {
        uiScript.instrunctionPanel.SetActive(true);
        uiScript.controlsPanel.SetActive(true);
        uiScript.uiPanel.SetActive(false);
        uiScript.optionPanel.SetActive(false);
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}
