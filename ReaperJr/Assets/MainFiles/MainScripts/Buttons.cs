using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Buttons : ReaperJr
{
    private void OnMouseOver()
    {
        _GAME.playerActive = false;
    }

    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        _GAME.Restart();
        GameEvents.ReportGameStateChange(GameState.RESUME);
    }

    public void Return()
    {
        GameEvents.ReportGameStateChange(GameState.RESUME);
    }

    public void Menu()
    {
        GameEvents.ReportGameStateChange(GameState.MENU);
        _UI.instrunctionPanel.SetActive(true);
        _UI.controlsInfoPanel.SetActive(true);
        _UI.uiInfoPanel.SetActive(false);
        _UI.optionPanel.SetActive(false);
    }

    public void ControlsPanel()
    {
        _UI.controlsInfoPanel.SetActive(true);
        _UI.uiInfoPanel.SetActive(false);
    }
    
    public void UIsPanel()
    {
        _UI.controlsInfoPanel.SetActive(false);
        _UI.uiInfoPanel.SetActive(true);
    }

    public void OptionPanel()
    {
        _UI.instrunctionPanel.SetActive(false);
        _UI.optionPanel.SetActive(true);
    }

    public void InstructionPanel()
    {
        _UI.instrunctionPanel.SetActive(true);
        _UI.controlsInfoPanel.SetActive(true);
        _UI.uiInfoPanel.SetActive(false);
        _UI.optionPanel.SetActive(false);
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}
