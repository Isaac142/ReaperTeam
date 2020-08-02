﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using UnityEngine.SceneManagement;

public enum HintForActions {DEFAULT, CANHOLD, RELEASING, SWITCH, OPEN, COLLECTSOULS, COLLECTITEMS, HEAVYOBJNOTE, REQUIRKEY, DISTANCEREQUIRED}

public class UIManager : Singleton<UIManager>
{
    [HideInInspector]
    public HintForActions currInfo;
    public float fadeInTime = 0.5f;
    public float fadeOutTime = 0.2f;
    public Ease fadeInEase;
    public Ease fadeOutEase;

    [Header("InGameUI")]

    public Text timerCount;
    public Image timer;

    public List<Image> souls = new List<Image>();
    [HideInInspector]
    public List<SoulType> currSouls = new List<SoulType>();
    public GameObject soulPanel;
    public Text totalSoulNo;
    public GameObject infoPanel;
    public Text itemName, itemType, itemDescription;
    public Slider energyBar;
    public Image abilityCD;
    public GameObject scytheMasks, abilityMask;  //scythe icon masks
    public GameObject hintsPanel;
    public TextMeshProUGUI hint1; //keyboard input
    public TextMeshProUGUI hint2; // mouse input
    public TextMeshProUGUI hint3; //object notes
    public GameObject keyItemPanel;
    public List<Image> keyItems = new List<Image>();
    [HideInInspector]
    public List<KeyItem> currKeyItems = new List<KeyItem>();

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
        StartSetUI();
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

    public void StartSetUI()
    {
        energyBar.maxValue = _GAME.maxEnergy;
        energyBar.minValue = 0f;

        CloseAllPanels();
        DisableSoulIcons();
        DisableKeyItems();
        hintsPanel.GetComponent<CanvasGroup>().alpha = 1;
        SetHintPanel();

        abilityMask.SetActive(false);
        scytheMasks.SetActive(false);

        infoPanel.SetActive(false);
        DOTween.SetTweensCapacity(2000, 100);
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
        InstantOffPanel(hintsPanel);
        InstantOffPanel(pausePanel);
        InstantOffPanel(gameOverPanel);
        InstantOffPanel(menuPanel);
        InstantOffPanel(wonPanel);
        InstantOffPanel(deadPanel);
        InstantOffPanel(hintsPanel);
    }

    public void DisableSoulIcons()
    {
        FadeOutPanel(soulPanel);
        foreach (Image soulIcon in souls)
        {
            soulIcon.sprite = null;
            soulIcon.enabled = false;
        }
    }

    public void DisableKeyItems()
    {
        FadeOutPanel(keyItemPanel);
        foreach (Image keyitem in keyItems)
        {
            keyitem.sprite = null;
            keyitem.enabled = false;
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
            abilityMask.SetActive(false);
            scytheMasks.SetActive(false);
        }
        else
        {
            abilityMask.SetActive(true);
            scytheMasks.SetActive(true);
        }
    }

    void OnScytheThrown(bool scytheThrow)
    {
        if (scytheThrow)
            scytheMasks.SetActive(true);
        else
            scytheMasks.SetActive(false);
    }   

    private void OnEnable()
    {
        GameEvents.OnGameStateChange += OnGameStateChange;
        GameEvents.OnScytheEquipped += OnScytheEquipped;
        GameEvents.OnHintShown += OnHintShown;
        GameEvents.OnScytheThrow += OnScytheThrown;
        GameEvents.OnKeyItemCollected += OnKeyItemCOllected;
        GameEvents.OnSoulCollected += OnSoulCollected;
    }

    private void OnDisable()
    {
        GameEvents.OnGameStateChange -= OnGameStateChange;
        GameEvents.OnScytheEquipped -= OnScytheEquipped;
        GameEvents.OnHintShown -= OnHintShown;
        GameEvents.OnScytheThrow -= OnScytheThrown;
        GameEvents.OnKeyItemCollected -= OnKeyItemCOllected;
        GameEvents.OnSoulCollected -= OnSoulCollected;
    }

    #region Button Press
    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        _GAME.ResetGame();
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

            if (currSouls[i].isCollected)
                souls[i].color = Color.gray;
            else
                souls[i].color = Color.white;
        }
        //UpdateSouls();
    }

    //public void UpdateSouls()
    //{
    //    for (int i = 0; i < currSouls.Count; i++)
    //    {
    //        if (currSouls[i].isCollected)
    //        {
    //            //souls[i].color = Color.gray;
    //            souls[i].DOColor(Color.red, 0.5f);
    //            souls[i].rectTransform.DOScale(Vector3.one * 1.5f, 0.5f);
    //            StartCoroutine(ReturnSouldIcon(souls[i]));
    //        }

    //        else
    //            souls[i].color = Color.white;
    //    }
    //}

    public void OnSoulCollected(SoulType soul)
    {
        for (int i = 0; i < currSouls.Count; i++)
        {
            if (currSouls[i] == soul)
            {
                souls[i].DOColor(Color.red, 0.5f);
                souls[i].rectTransform.DOScale(Vector3.one * 1.5f, 0.5f);
                StartCoroutine(ReturnSoulIcon(souls[i]));
            }
        }
    }

    IEnumerator ReturnSoulIcon(Image _soul)
    {
        yield return new WaitForSeconds(0.5f);
        _soul.rectTransform.DOScale(Vector3.one * 1f, 1f);
        _soul.DOColor(Color.grey, 1f);
    }

    public void SetKeyItemPanel(List<KeyItem> keyItemSprite)
    {
        FadeInPanel(keyItemPanel);

        currKeyItems = keyItemSprite;
        foreach (Image im in keyItems)
        {
            im.enabled = false;
        }

        for (int i = 0; i < currKeyItems.Count; i++)
        {
            keyItems[i].sprite = currKeyItems[i].itemSprite;
            keyItems[i].enabled = true;
            if (currKeyItems[i].isCollected)
                keyItems[i].color = Color.white;
            else
                keyItems[i].color = Color.gray;
        }
    }

    public void OnKeyItemCOllected(KeyItem key)
    {

        for (int i = 0; i < currKeyItems.Count; i++)
        {
            if (currKeyItems[i] == key)
            {
                keyItems[i].DOColor(Color.white , 0.5f);
                keyItems[i].rectTransform.DOScale(Vector3.one * 1.5f, 0.5f);
                StartCoroutine(ReturnKeyItemIcon(keyItems[i]));
            }
        }
    }

    IEnumerator ReturnKeyItemIcon(Image keyItem)
    {
        yield return new WaitForSeconds(0.5f);
        keyItem.rectTransform.DOScale(Vector3.one * 1f, 1f);
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

    public void SetHintPanel()
    {
        hint1.text = null;
        hint1.color = Color.white;
        hint2.text = null;
        hint2.color = Color.white;
        hint3.text = null;
        hint3.color = Color.white;
    }
    
    public void OnHintShown (HintForActions action)
    {
        currInfo = action;
        switch(action)
        {
            case HintForActions.CANHOLD:
                hint1.text = "Press E key to Hold Object in front.";
                break;
            case HintForActions.RELEASING:
                hint1.text = "Press E key to Release Object in front.";
                break;
            case HintForActions.HEAVYOBJNOTE:
                hint1.text = "Press E key to Release Object in front.";
                hint2.text = "You can ONLY drag or push this object.";
                hint2.color = new Color(237f / 255f, 195f / 255f, 1);
                break;
            case HintForActions.SWITCH:
                hint3.text = "Right click to initiating it";
                break;
            case HintForActions.OPEN:
                hint2.text = "Right click to open it";
                break;
            case HintForActions.COLLECTSOULS:
                hint2.text = "Right click to collect the soul(s).";
                hint3.text = "Don't collect fake soul(s).";
                hint3.color = new Color(237f / 255f, 195f/ 255f, 1f);
                break;
            case HintForActions.COLLECTITEMS:
                hint2.text = "Right click to collect the object(s).";
                break;
            case HintForActions.REQUIRKEY:
                hint3.text = "Requir Keyitems!";
                hint3.color = new Color(237f / 255f, 195f / 255f, 1);
                break;
            case HintForActions.DISTANCEREQUIRED:
                hint3.text = "You need to get closer.";
                hint3.color = new Color(237f / 255f, 195f / 255f, 1);
                break;
        }
    }
}

