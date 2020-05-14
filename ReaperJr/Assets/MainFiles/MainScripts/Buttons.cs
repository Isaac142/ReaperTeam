using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Buttons : MonoBehaviour
{
    public UpdateUI uiScript;

    private void Awake()
    { 
        if(uiScript == null)
            uiScript = FindObjectOfType<Canvas>().GetComponent<UpdateUI>();
    }

    private void OnMouseOver()
    {
        GameManager.Instance.playerActive = false;
    }

    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        GameManager.Instance.isPaused = false;
        GameManager.Instance.Energy = GameManager.Instance.maxEnergy;
        GameManager.Instance.Timer = GameManager.Instance.maxTimerInSeconds;
        GameManager.Instance.dead = false;
        GameManager.Instance.gameOver = false;
        GameManager.Instance.wonGame = false;
        GameManager.Instance.holdingLightObject = false;
        GameManager.Instance.isHolding = false;
        GameManager.Instance.canHold = true;
        GameManager.Instance.onSpecialGround = false;
        GameManager.Instance.pausePanel = false;
        GameManager.Instance.menuPanel = false;
        GameManager.Instance.totalSoulNo = 0;
        Time.timeScale = 1;
    }

    public void Return()
    {
        GameManager.Instance.pausePanel = false;
        GameManager.Instance.isPaused = false;
        GameManager.Instance.menuPanel = false;
    }

    public void Menu()
    {
        GameManager.Instance.menuPanel = true;
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
