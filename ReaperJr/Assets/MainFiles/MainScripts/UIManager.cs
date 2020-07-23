﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public enum HintForActions {DEFAULT, CANHOLD, RELEASING, SWITCH, OPENBOX, MOVABLEOPENABLEBOX , COLLECTSOULS, COLLECTITEMS}

public class UIManager : Singleton<UIManager>
{
    public HintForActions currInfo;
    public float fadeInTime = 0.5f;
    public float fadeOutTime = 0.2f;
    public Ease fadeInEase;
    public Ease fadeOutEase;

    [Header("InGameUI")]

    public Text timerCount;
    public Image timer;

    public List<Image> souls = new List<Image>();
    public List<SoulType> currSouls = new List<SoulType>();
    public GameObject soulPanel;
    public Text totalSoulNo;
    public GameObject infoPanel;
    public Text itemName, itemType, itemDescription;
    public Slider energyBar;
    public Image abilityCD;
    public GameObject[] masks;  //scythe icon masks
    public GameObject hintsPanel;
    public TextMeshProUGUI hint1; //keyboard input
    public TextMeshProUGUI hint2; // mouse input

    [Header("GameStatePanels")]
    public GameObject inGamePanel;  //in game UI display (timer, scythe icons and souls)
    public GameObject menuPanel;
    public GameObject instructionPanel;
    public GameObject controlsInfoPanel;
    public GameObject optionPanel;
    public GameObject uiInfoPanel;
    public GameObject pausePanel;
    public GameObject gameOverPanel;
    public GameObject wonPanel;
    public GameObject deadPanel;

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
    public void FadeOutAllPanels()
    {
        FadeOutPanel(inGamePanel);
        FadeOutPanel(pausePanel);
        FadeOutPanel(gameOverPanel);
        FadeOutPanel(menuPanel);
        FadeOutPanel(wonPanel);
        FadeOutPanel(deadPanel);
    }
    public void CloseAllPanels()
    {
        //InstantOffPanel(inGamePanel);
        InstantOffPanel(hintsPanel);
        InstantOffPanel(pausePanel);
        InstantOffPanel(gameOverPanel);
        InstantOffPanel(menuPanel);
        InstantOffPanel(wonPanel);
        InstantOffPanel(deadPanel);
    }

    public void DisableSoulIcons()
    {
        FadeOutPanel(hintsPanel);
        foreach (Image soulIcon in souls)
        {
            soulIcon.sprite = null;
            soulIcon.enabled = false;
        }
    }

    void OnGameStateChange(GameState state)
    {
        FadeOutAllPanels();
        switch (state)
        {
            case GameState.INGAME:
                FadeInPanel(inGamePanel);
                break;
            case GameState.PAUSED:
                FadeInPanel(pausePanel);
                break;
            case GameState.MENU:
                FadeInPanel(menuPanel);
                break;
            case GameState.GAMEOVER:
                FadeInPanel(gameOverPanel);
                break;
            case GameState.WON:
                FadeInPanel(wonPanel);
                break;
            case GameState.DEAD:
                FadeInPanel(deadPanel);
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

    void OnSoulCollected(SoulType soulCollected)
    {
        //UpdateSouls();
    }

    private void OnEnable()
    {
        GameEvents.OnGameStateChange += OnGameStateChange;
        GameEvents.OnScytheEquipped += OnScytheEquipped;
        GameEvents.OnSoulCollected += OnSoulCollected;
        GameEvents.OnHintShown += OnHintShown;
    }

    private void OnDisable()
    {
        GameEvents.OnGameStateChange -= OnGameStateChange;
        GameEvents.OnScytheEquipped -= OnScytheEquipped;
        GameEvents.OnSoulCollected -= OnSoulCollected;
        GameEvents.OnHintShown -= OnHintShown;
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
        instructionPanel.GetComponent<CanvasGroup>().alpha = 1;
        controlsInfoPanel.GetComponent<CanvasGroup>().alpha = 1;
        uiInfoPanel.GetComponent<CanvasGroup>().alpha = 0;
        optionPanel.GetComponent<CanvasGroup>().alpha = 0;
    }

    public void ControlsPanel()
    {
        FadeOutPanel(uiInfoPanel);
        FadeInPanel(controlsInfoPanel);
    }

    public void UIsPanel()
    {
        FadeOutPanel(controlsInfoPanel);
        FadeInPanel(uiInfoPanel);

    }

    public void OptionPanel()
    {
        FadeOutPanel(instructionPanel);
        FadeInPanel(optionPanel);
    }

    public void InstructionPanel()
    {
        FadeOutPanel(optionPanel);
        FadeInPanel(instructionPanel);
        controlsInfoPanel.GetComponent<CanvasGroup>().alpha = 1;
        uiInfoPanel.GetComponent<CanvasGroup>().alpha = 0;
        optionPanel.GetComponent<CanvasGroup>().alpha = 0;

    }

    public void ExitGame()
    {
        Application.Quit();
    }
    #endregion

    public void SetSouls(List<SoulType> _souls)
    {
        FadeInPanel(soulPanel);
        //currSouls.Clear();
        currSouls = _souls;
        foreach (Image im in souls)
        {
            im.enabled = false;
        }

        for (int i = 0; i < currSouls.Count; i++)
        {
            souls[i].sprite = currSouls[i].soulIcon;
            souls[i].enabled = true;
        }
        UpdateSouls();
    }

    public void UpdateSouls()
    {
        for (int i = 0; i < currSouls.Count; i++)
        {
            if (currSouls[i].isCollected)
            {
                //souls[i].color = Color.gray;
                souls[i].DOColor(Color.red, 0.5f);
                souls[i].rectTransform.DOScale(Vector3.one * 1.5f, 0.5f);
                StartCoroutine(ReturnSouldIcon(souls[i]));
            }

            else
                souls[i].color = Color.white;
        }

    }

    IEnumerator ReturnSouldIcon(Image _soul)
    {
        yield return new WaitForSeconds(0.5f);
        _soul.rectTransform.DOScale(Vector3.one * 1f, 1f);
        _soul.DOColor(Color.grey, 1f);
    }

    public void FadeInPanel(GameObject panel)
    {
        CanvasGroup cvg = panel.GetComponent<CanvasGroup>();
        cvg.DOFade(1, fadeInTime).SetEase(fadeInEase).SetUpdate(true);
        cvg.interactable = true;
        cvg.blocksRaycasts = true;
    }
    public void FadeOutPanel(GameObject panel)
    {
        CanvasGroup cvg = panel.GetComponent<CanvasGroup>();
        cvg.DOFade(0, fadeOutTime).SetEase(fadeOutEase).SetUpdate(true);
        cvg.interactable = false;
        cvg.blocksRaycasts = false;
    }

    void InstantInPanel(GameObject panel)
    {
        CanvasGroup cvg = panel.GetComponent<CanvasGroup>();
        cvg.alpha = 1;
        cvg.interactable = true;
        cvg.blocksRaycasts = true;
    }

    void InstantOffPanel(GameObject panel)
    {
        CanvasGroup cvg = panel.GetComponent<CanvasGroup>();
        cvg.alpha = 0;
        cvg.interactable = false;
        cvg.blocksRaycasts = false;
    }

    private void SetHints()
    {
        CanvasGroup cvg = hintsPanel.GetComponent<CanvasGroup>();
        cvg.DOFade(1f, 0.5f).SetEase(fadeInEase).SetUpdate(true);
        hint1.enabled = true;
        hint2.enabled = true;
    }

    public void OnHintShown (HintForActions action)
    {
        currInfo = action;
        switch(action)
        {
            case HintForActions.DEFAULT:
                FadeOutPanel(hintsPanel);
                break;
            case HintForActions.CANHOLD:
                SetHints();
                hint1.text = "Press E key to Hold Object in front.";
                hint2.enabled = false;
                break;
            case HintForActions.RELEASING:
                SetHints();
                hint1.text = "Press E key to Release Object in front.";
                hint2.enabled = false;
                break;
            case HintForActions.SWITCH:
                SetHints();
                hint2.text = "Right click on the switch to initiating the object";
                hint1.enabled = false;
                break;
            case HintForActions.OPENBOX:
                SetHints();
                hint2.text = "Right click to open the box in front";
                hint1.enabled = false;
                break;
            case HintForActions.MOVABLEOPENABLEBOX:
                hint2.enabled = true;
                hint2.text = "Right click to open the box in front.";
                break;
            case HintForActions.COLLECTSOULS:
                SetHints();
                hint1.text = "Right click to collect the soul(s).";
                hint2.text = "Don't collect fake soul(s).";
                break;
            case HintForActions.COLLECTITEMS:
                SetHints();
                hint1.enabled = false;
                hint2.text = "Right click to collect the object(s).";
                break;
        }
    }
}

