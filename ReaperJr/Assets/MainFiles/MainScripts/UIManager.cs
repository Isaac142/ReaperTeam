﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : Singleton<UIManager>
{
    [Header("InGameUI")]

    public Text timerCount;
    public Image timer;

    public List<Image> souls = new List<Image>();
    public List<Image> soulMasks = new List<Image>();
    public Text totalSoulNo;
    public GameObject infoPanel;
    public Text itemName, itemType, itemDescription;
    public Slider energyBar;
    public Image abilityCD;
    public GameObject[] masks;  //scythe icon masks

    [Header("GameStatePanels")]
    public GameObject inGamePanel;  //in game UI display (timer, scythe icons and souls)
    public GameObject menuPanel;
    public GameObject instrunctionPanel;
    public GameObject controlsInfoPanel;
    public GameObject optionPanel;
    public GameObject uiInfoPanel;
    public GameObject pausePanel;
    public GameObject gameOverPanel;
    public GameObject wonPanel;

    [HideInInspector]
    public bool instructionOn = false, controlsOn = false, UIsOn = false, optionOn = false;

    private string FormatTimeMMSS(float timeInseconds)
    {
        float fraction = timeInseconds * 100;
        return string.Format("{0:00}:{1:00}:{2:00}", Mathf.FloorToInt(timeInseconds / 60), Mathf.FloorToInt(timeInseconds % 60), fraction % 100);
    }

    private string FormatTimeSSMilS(float timeInseconds)
    {
        float fraction = timeInseconds * 100;
        return string.Format("{0:00}:{1:00}", Mathf.FloorToInt(timeInseconds % 60), fraction % 100);
    }

    // Start is called before the first frame update
    void Start()
    {
        energyBar.maxValue = _GAME.maxEnergy;
        energyBar.minValue = 0f;

        CloseAllPanels();
        DisableSoulIcons();

        foreach (GameObject pic in masks)
            pic.SetActive(false);

        infoPanel.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        #region TimerDisplay
        //Timer display set up
        timerCount.text = FormatTimeMMSS(_GAME.Timer);
        timer.fillAmount = _GAME.Timer / _GAME.maxTimerInSeconds;

        if (_GAME.Timer < _GAME.warningTimeInSeconds)  //Timer low display
        {
            //texture set up
            timerCount.text = FormatTimeSSMilS(_GAME.Timer);
            timerCount.GetComponent<Text>().color = Color.yellow;
            timerCount.fontSize = 45;

            //clock set up

            timer.fillAmount = _GAME.Timer / _GAME.maxTimerInSeconds;
            timer.GetComponent<Image>().color = Color.red;
        }
        #endregion

        energyBar.value = _GAME.Energy;

       

        abilityCD.fillAmount = _GAME.CDTimer / _GAME.coolDown;

        totalSoulNo.text = _GAME.totalSoulNo.ToString();
    }
    public void CloseAllPanels()
    {
        inGamePanel.SetActive(false);
        pausePanel.SetActive(false);
        gameOverPanel.SetActive(false);
        menuPanel.SetActive(false);
        wonPanel.SetActive(false);
    }

    public void DisableSoulIcons()
    {
        foreach(Image soulIcon in souls)
        {
            soulIcon.sprite = null;
            soulIcon.enabled = false;
        }

        foreach (Image soulIconMask in soulMasks)

        {
            soulIconMask.sprite = null;
            soulIconMask.enabled = false;
        }
    }

    void OnGameStateChange(GameState state)
    {
        CloseAllPanels();
         switch (state)
        {
            case GameState.INGAME:
                inGamePanel.SetActive(true);
                break;
            case GameState.PAUSED:
                pausePanel.SetActive(true);
                break;
            case GameState.MENU:
                menuPanel.SetActive(true);
                break;
            case GameState.GAMEOVER:
                gameOverPanel.SetActive(true);
                break;
            case GameState.WON:
                wonPanel.SetActive(true);
                break;
        }
    }

    void OnScytheEquipped(bool scythe)
    {
        if (scythe)
        {
            foreach (GameObject pic in masks)
                pic.SetActive(false);
        }
        else
        {
            foreach (GameObject pic in masks)
                pic.SetActive(true);
        }
    }

    private void OnEnable()
    {
        GameEvents.OnGameStateChange += OnGameStateChange;
        GameEvents.OnScytheEquipped += OnScytheEquipped;
    }

    private void OnDisable()
    {
        GameEvents.OnGameStateChange -= OnGameStateChange;
        GameEvents.OnScytheEquipped -= OnScytheEquipped;
    }

    #region Button Press
    public void Restart()
    {
        
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
        instrunctionPanel.SetActive(true);
        controlsInfoPanel.SetActive(true);
        uiInfoPanel.SetActive(false);
        optionPanel.SetActive(false);
    }

    public void ControlsPanel()
    {
        controlsInfoPanel.SetActive(true);
        uiInfoPanel.SetActive(false);
    }

    public void UIsPanel()
    {
        controlsInfoPanel.SetActive(false);
        uiInfoPanel.SetActive(true);
    }

    public void OptionPanel()
    {
        instrunctionPanel.SetActive(false);
        optionPanel.SetActive(true);
    }

    public void InstructionPanel()
    {
        instrunctionPanel.SetActive(true);
        controlsInfoPanel.SetActive(true);
        uiInfoPanel.SetActive(false);
        optionPanel.SetActive(false);
    }

    public void ExitGame()
    {
        Application.Quit();
    }
    #endregion
}

