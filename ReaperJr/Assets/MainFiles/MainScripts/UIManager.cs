using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using UnityEngine.SceneManagement;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public enum HintForMovingBoxes { DEFAULT, CANHOLD, RELEASING, HEAVYOBJNOTE }
public enum HintForItemCollect { DEFAULT, COLLECTSOULS, COLLECTITEMS, FAKESOULWARNING }
public enum HintForInteraction { DEFAULT, SWITCH, OPEN, REQUIRKEY, DISTANCEREQUIRED, KEYITEM }

public class UIManager : Singleton<UIManager>
{

    public Texture2D cursor;

    [HideInInspector]
    public HintForMovingBoxes currMovingInfo;
    [HideInInspector]
    public HintForItemCollect currCollectInfo;
    [HideInInspector]
    public HintForInteraction currInteractInfo;
    public float fadeInTime = 0.5f;
    public float fadeOutTime = 0.2f;
    public Ease fadeInEase;
    public Ease fadeOutEase;
    public Volume volume;
    ColorAdjustments colorAdj;

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
    public TextMeshProUGUI MovingObjHint; //moving object hint
    public TextMeshProUGUI CollectingHint; // collecting object hint
    public TextMeshProUGUI InteractionHint1, InteractionHint2; //interact hint
    float oriFontSize;
    public GameObject keyItemPanel;
    public List<Image> keyItems = new List<Image>();
    [HideInInspector]
    public List<KeyItem> currKeyItems = new List<KeyItem>();
    public Image crosshairIcon;

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

    public Slider brightnessSlider, musicSlider, soundFXSlider;
    public Toggle musicToggle, soundFXToggle;

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
        if (cursor != null)
            Cursor.SetCursor(cursor, Vector2.zero, CursorMode.ForceSoftware);
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

        switch(_GAME.gameState)
        {
            case GameState.MENU:;
                colorAdj.postExposure.value = brightnessSlider.value;

                //_GAME.lightSource.intensity = brightnessSlider.value;
                _AUDIO.MuteMusic(musicToggle.isOn);
                _AUDIO.MusicVolume(musicSlider.value);
                _AUDIO.MuteSoundFX(soundFXToggle.isOn);
                _AUDIO.SoundFXVolume(soundFXSlider.value);
                break;
        }
    }

    public void StartSetUI()
    {
        if(volume != null)
            colorAdj.active = volume.profile.TryGet(out colorAdj);
        energyBar.maxValue = _GAME.maxEnergy;
        energyBar.minValue = 0f;

        oriFontSize = CollectingHint.fontSize;

        CloseAllPanels();
        DisableSoulIcons();
        DisableKeyItems();
        hintsPanel.GetComponent<CanvasGroup>().alpha = 1;
        SetHintPanel();

        abilityMask.SetActive(false);
        scytheMasks.SetActive(false);

        infoPanel.SetActive(false);
        DOTween.SetTweensCapacity(2000, 100);

        brightnessSlider.minValue = -1;
        brightnessSlider.maxValue = 1f;
        musicSlider.minValue = 0;
        musicSlider.maxValue = 5;
        soundFXSlider.minValue = 0;
        soundFXSlider.maxValue = 5;

        OptionPanelDefault();
    }

    public void OptionPanelDefault()
    {
        
        brightnessSlider.value = 0.5f;
        soundFXSlider.value = 1;
        musicSlider.value = 1;
        musicToggle.isOn = false;
        soundFXToggle.isOn = false;
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
        GameEvents.OnMovableHintShown += OnMovableHintShown;
        GameEvents.OnScytheThrow += OnScytheThrown;
        GameEvents.OnKeyItemCollected += OnKeyItemCOllected;
        GameEvents.OnSoulCollected += OnSoulCollected;
        GameEvents.OnCollectHintShown += OnCollectHintShown;
        GameEvents.OnInteractHintShown += OnInterActionHintShown;
        GameEvents.OnCrossHairOut += OnCrossHairOut;
    }

    private void OnDisable()
    {
        GameEvents.OnGameStateChange -= OnGameStateChange;
        GameEvents.OnScytheEquipped -= OnScytheEquipped;
        GameEvents.OnMovableHintShown -= OnMovableHintShown;
        GameEvents.OnScytheThrow -= OnScytheThrown;
        GameEvents.OnKeyItemCollected -= OnKeyItemCOllected;
        GameEvents.OnSoulCollected -= OnSoulCollected;
        GameEvents.OnCrossHairOut -= OnCrossHairOut;
        GameEvents.OnCollectHintShown -= OnCollectHintShown;
        GameEvents.OnInteractHintShown -= OnInterActionHintShown;
    }

    void OnCrossHairOut(bool crosshair)
    {
        if (crosshair)
            crosshairIcon.color = Color.white;
        else
            crosshairIcon.color = Color.gray;
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
        InstructionPanel();
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
                keyItems[i].DOColor(Color.white, 0.5f);
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
        GameEvents.ReportMovableHintShown(HintForMovingBoxes.DEFAULT);
        GameEvents.ReportInteractHintShown(HintForInteraction.DEFAULT);
        GameEvents.ReportCollectHintShown(HintForItemCollect.DEFAULT);
    }

    public void OnMovableHintShown(HintForMovingBoxes action)
    {
        currMovingInfo = action;
        switch (action)
        {
            case HintForMovingBoxes.DEFAULT:
                FadeOutText(MovingObjHint);
                break;
            case HintForMovingBoxes.CANHOLD:
                MovingObjHint.text = "<color=#FFFFFF> Press E key to Hold Object in front.";
                FadeInText(MovingObjHint);
                break;
            case HintForMovingBoxes.RELEASING:
                MovingObjHint.text = "<color=#FFFFFF> Press E key to Release Object in front.";
                FadeInText(MovingObjHint);
                break;
            case HintForMovingBoxes.HEAVYOBJNOTE:
                MovingObjHint.text = "<color=#FFFFFF> Press <b> E key </b> to Release Object in front. \n <color=#EDC3FF> You can ONLY drag or push this object.";
                FadeInText(MovingObjHint);
                break;
        }
    }

    public void OnCollectHintShown(HintForItemCollect action)
    {
        currCollectInfo = action;
        switch (action)
        {
            case HintForItemCollect.DEFAULT:
                FadeOutText(CollectingHint);
                break;
            case HintForItemCollect.COLLECTSOULS:
                CollectingHint.text = "<color=#FFFFFF> Right click to collect the soul(s).\n <color=#EDC3FF> Don't collect fake soul(s).";
                FadeInText(CollectingHint);
                break;
            case HintForItemCollect.COLLECTITEMS:
                CollectingHint.text = "<color=#FFFFFF> Right click to collect the object(s).";
                FadeInText(CollectingHint);
                break;
            case HintForItemCollect.FAKESOULWARNING:

                CollectingHint.text = "<color=red> <size=80> Fake Soul! RUN!!!";
                FadeInText(CollectingHint);
                break;
        }
    }

    public void OnInterActionHintShown(HintForInteraction action)
    {
        currInteractInfo = action;
        switch (action)
        {
            case HintForInteraction.DEFAULT:
                FadeOutText(InteractionHint1);
                FadeOutText(InteractionHint2);
                break;
            case HintForInteraction.SWITCH:
                InteractionHint1.text = "<color=#FFFFFF> Right click initiating it";
                FadeInText(InteractionHint1);
                break;
            case HintForInteraction.OPEN:
                InteractionHint2.text = "<color=#FFFFFF> Right click to open it";
                FadeInText(InteractionHint2);
                break;
            case HintForInteraction.REQUIRKEY:
                InteractionHint1.text = "<color=#EDC3FF> Require Key Items!";
                FadeInText(InteractionHint1);
                break;
            case HintForInteraction.DISTANCEREQUIRED:
                InteractionHint1.text = "<color=#EDC3FF> You need to get closer.";
                FadeInText(InteractionHint1);
                break;
            case HintForInteraction.KEYITEM:
                InteractionHint2.text = "<color=red> This is a Key Item!";
                FadeInText(InteractionHint2);
                break;
        }
    }

    void FadeInText(TextMeshProUGUI textUI)
    {
        textUI.DOFade(1, fadeInTime);

    }
    void FadeOutText(TextMeshProUGUI textUI)
    {
        textUI.DOFade(0, fadeOutTime);
    }
}

