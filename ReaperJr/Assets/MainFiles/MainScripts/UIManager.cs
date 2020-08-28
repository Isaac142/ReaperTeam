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
public enum HintForItemCollect { DEFAULT, COLLECTSOULS, COLLECTITEMS, FAKESOULWARNING, KEYLIMIT }
public enum HintForInteraction { DEFAULT, SWITCH, RETURN, REQUIRKEY, DISTANCEREQUIRED, KEYITEM, MOUSETRAP, RETURNSOULS, FINISH, JAR, CROUCH }

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

    public TextMeshProUGUI timerCount;
    public Image timer;

    public List<Image> souls = new List<Image>();
    [HideInInspector]
    public List<SoulType> currSouls = new List<SoulType>();
    public GameObject soulPanel;
    public Text totalSoulNo;
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
    public Animator storyAnim, openningAnim;
    public TextMeshPro checkPointInfo;
    public GameObject timeChangePanel;
    public TextMeshProUGUI timeChangeMessage;

    [Header("GameStatePanels")]
    public GameObject inGamePanel;  //in game UI display (timer, scythe icons and souls)
    public GameObject titlePanel;
    public GameObject storyPanel;
    public GameObject openningPanel;
    public GameObject menuPanel;
    public GameObject instructionPanel;
    public GameObject controlsInfoPanel;
    public GameObject optionPanel;
    public GameObject uiInfoPanel;
    public GameObject creditPanel;
    public GameObject pausePanel;
    public GameObject gameOverPanel;
    public GameObject wonPanel;
    public GameObject deadPanel;
    public GameObject roomClearPanel;

    public Slider brightnessSlider, musicSlider, soundFXSlider;
    public Toggle musicToggle, soundFXToggle;
    private int currSlide = 0;
    private float openningTimer = 0;
    [HideInInspector]
    public bool menuTest = false;

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
        currSlide = 0;
        if (checkPointInfo != null)
            DontDestroyOnLoad(checkPointInfo.gameObject);

        OptionPanelDefault();
    }

    // Update is called once per frame
    void Update()
    {
        colorAdj.postExposure.value = brightnessSlider.value;
        _AUDIO.MuteMusic(musicToggle.isOn);
        _AUDIO.MusicVolume(musicSlider.value);
        _AUDIO.MuteSoundFX(soundFXToggle.isOn);
        _AUDIO.SoundFXVolume(soundFXSlider.value);

        switch (_GAME.gameState)
        {
            case GameState.MENU:
                if (currSlide == 8)
                    currSlide = 0;
                break;
            case GameState.OPENNING:
                openningTimer += Time.deltaTime;

                if (Input.GetKeyDown(KeyCode.Escape))
                    openningTimer = 54f;

                if (openningTimer >= 54f)
                    Restart();
                break;

            case GameState.INGAME:
                #region TimerDisplay
                //Timer display set up
                timerCount.text = FormatTimeMMSS(_GAME.Timer);
                timer.fillAmount = _GAME.Timer / _GAME.maxTimerInSeconds;

                if (_GAME.Timer < _GAME.warningTimeInSeconds)  //Timer low display
                {
                    //texture set up
                    timerCount.text = FormatTimeSSMilS(_GAME.Timer);
                    timerCount.color = Color.yellow;
                    timerCount.fontSize = 45f;
                    _AUDIO.PlayMusic("TimeStressed");
                    //clock set up

                    timer.fillAmount = _GAME.Timer / _GAME.maxTimerInSeconds;
                    timer.GetComponent<Image>().color = Color.red;
                }

                else
                {
                    timerCount.color = Color.white;
                    timerCount.fontSize = 35f;
                    timer.GetComponent<Image>().color = Color.cyan;
                }

                #endregion
                energyBar.value = _GAME.Energy;

                abilityCD.fillAmount = _GAME.CDTimer / _GAME.coolDown;

                totalSoulNo.text = _GAME.totalSoulNo.ToString();
                break;

            case GameState.GAMEOVER:
                if(gameOverPanel.GetComponent<Animator>() != null)
                {
                    if (Input.GetKeyDown(KeyCode.Escape))
                        gameOverPanel.GetComponent<Animator>().SetTrigger("Skip");
                }
                break;
            case GameState.WON:
                if (wonPanel.GetComponent<Animator>() != null)
                {
                    if (Input.GetKeyDown(KeyCode.Escape))
                        wonPanel.GetComponent<Animator>().SetTrigger("Skip");
                }
                break;
        }
    }

    public void StartSetUI()
    {
        if(volume != null)
            colorAdj.active = volume.profile.TryGet<ColorAdjustments>(out colorAdj);
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
        DOTween.SetTweensCapacity(2000, 100);

        brightnessSlider.minValue = -1;
        brightnessSlider.maxValue = 1f;
        musicSlider.minValue = 0;
        musicSlider.maxValue = 5;
        soundFXSlider.minValue = 0;
        soundFXSlider.maxValue = 5;
    }

    public void OptionPanelDefault()
    {
        
        brightnessSlider.value = 0f;
        soundFXSlider.value = 1;
        musicSlider.value = 1;
        musicToggle.isOn = false;
        soundFXToggle.isOn = false;
    }


    public void FadeOutAllPanels()
    {
        FadeOutPanel(titlePanel);
        FadeOutPanel(storyPanel);
        FadeOutPanel(openningPanel);
        FadeOutPanel(inGamePanel);
        FadeOutPanel(pausePanel);
        FadeOutPanel(gameOverPanel);
        FadeOutPanel(menuPanel);
        FadeOutPanel(wonPanel);
        FadeOutPanel(deadPanel);
        FadeOutPanel(roomClearPanel);
    }
    public void CloseAllPanels()
    {
        InstantOffPanel(titlePanel);
        InstantOffPanel(storyPanel);
        InstantOffPanel(openningPanel);
        InstantOffPanel(inGamePanel);
        InstantOffPanel(hintsPanel);
        InstantOffPanel(pausePanel);
        InstantOffPanel(gameOverPanel);
        InstantOffPanel(menuPanel);
        InstantOffPanel(wonPanel);
        InstantOffPanel(deadPanel);
        InstantOffPanel(hintsPanel);
        InstantOffPanel(roomClearPanel);
        InstantOffPanel(timeChangePanel);
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
            case GameState.TITLE:
                FadeInPanel(titlePanel);
                break;
            case GameState.OPENNING:
                FadeInPanel(openningPanel);
                openningTimer = 0f;
                openningAnim.SetTrigger("Play");
                break;
            case GameState.INGAME:
                AnimationReset();
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
                if (gameOverPanel.GetComponent<Animator>() != null)
                    gameOverPanel.GetComponent<Animator>().SetTrigger("Play");
                break;
            case GameState.WON:
                FadeInPanel(wonPanel);
                if (wonPanel.GetComponent<Animator>() != null)
                    wonPanel.GetComponent<Animator>().SetTrigger("Play");
                break;
            case GameState.DEAD:
                FadeInPanel(deadPanel);
                break;
            case GameState.VICTORY:
                FadeInPanel(roomClearPanel);
                roomClearPanel.GetComponentInChildren<TextMeshProUGUI>().text = "All Souls Collected in the Room." 
                    + "\n <size=120> <color=#9C00FF> " + (_GAME.totalSoulNo - 1 ).ToString() + "<size=80> <color=white> to Collect.";
                break;
        }
    }

    void AnimationReset()  //reset ending animation
    {
        if (wonPanel.GetComponent<Animator>() != null)
            wonPanel.GetComponent<Animator>().SetTrigger("New");

        if (gameOverPanel.GetComponent<Animator>() != null)
            gameOverPanel.GetComponent<Animator>().SetTrigger("New");
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
        GameEvents.OnFallDeath += OnFallDeath;
        GameEvents.OnTimeChange += OnTimeChange;
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
        GameEvents.OnFallDeath -= OnFallDeath;
        GameEvents.OnTimeChange -= OnTimeChange;
    }

    void OnCrossHairOut(bool crosshair)
    {
        if (crosshair)
            crosshairIcon.color = Color.white;
        else
            crosshairIcon.color = Color.gray;
    }
    #region Button Press

    public void StartButton()
    {
        GameEvents.ReportGameStateChange(GameState.OPENNING);
    }

    public void OpenningNext()
    {
        storyAnim.SetInteger("Steps", currSlide + 1);
        currSlide ++;
    }

    public void Restart()
    {
        openningTimer = 54f;
        GameEvents.ReportOnMovingObject(false);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex, LoadSceneMode.Single);
        _GAME.ResetGame();
        GameEvents.ReportGameStateChange(GameState.RESUME);
    }

    public void Return()
    {
        GameEvents.ReportGameStateChange(GameState.RESUME);
    }

    public void Menu()
    {
        if (menuTest)
        {
            GameEvents.ReportGameStateChange(GameState.MENU);
            InstructionPanel();
        }
        else
            return;
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
        FadeOutPanel(storyPanel);
        FadeInPanel(optionPanel);
        FadeOutPanel(creditPanel);
    }

    public void InstructionPanel()
    {
        FadeOutPanel(optionPanel);
        FadeOutPanel(storyPanel);
        FadeOutPanel(creditPanel);
        FadeInPanel(instructionPanel);
        controlsInfoPanel.GetComponent<CanvasGroup>().alpha = 1;
        uiInfoPanel.GetComponent<CanvasGroup>().alpha = 0;
        optionPanel.GetComponent<CanvasGroup>().alpha = 0;

    }

    public void StoryPanel()
    {
        FadeOutPanel(instructionPanel);
        FadeOutPanel(optionPanel);
        FadeInPanel(storyPanel);
        FadeOutPanel(creditPanel);
    }

    public void CreditPanel()
    {
        FadeOutPanel(instructionPanel);
        FadeOutPanel(optionPanel);
        FadeInPanel(creditPanel);
        FadeOutPanel(storyPanel);
    }

    public void ExitGame()
    {
        Application.Quit();
    }
    #endregion

    public void SetSouls(List<SoulType> _souls)
    {
        FadeInPanel(soulPanel);
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
    }

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
        List<SoulType> collected = currSouls.FindAll(x => x.isCollected == true);

        if(currSouls.Count == collected.Count) //play room clear animation after collect all souls in a room.
        {
            FadeInPanel(roomClearPanel);
            _CAMERA.RoomClear();
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
        FadeOutText(MovingObjHint);
        currMovingInfo = action;
        switch (action)
        {
            case HintForMovingBoxes.DEFAULT:
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
        FadeOutText(CollectingHint);
        currCollectInfo = action;
        switch (action)
        {
            case HintForItemCollect.DEFAULT:
                break;
            case HintForItemCollect.COLLECTSOULS:
                CollectingHint.text = "<color=#FFFFFF> Right click to collect the soul(s).\n <color=#EDC3FF> Don't collect fake soul(s).";
                FadeInText(CollectingHint);
                break;
            case HintForItemCollect.COLLECTITEMS:
                CollectingHint.text = "<color=#FFFFFF> Right click to collect the object(s).";
                FadeInText(CollectingHint);
                break;
            case HintForItemCollect.KEYLIMIT:
                CollectingHint.text = "<color=#FFFFFF> You can ONLY carry ONE of these Items.";
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
        FadeOutText(InteractionHint1);
        FadeOutText(InteractionHint2);
        switch (action)
        {
            case HintForInteraction.DEFAULT:
                break;
            case HintForInteraction.SWITCH:
                InteractionHint1.text = "<color=#FFFFFF> Right click initiating it";
                FadeInText(InteractionHint1);
                break;
            case HintForInteraction.RETURN:
                InteractionHint2.text = "<color=#FFFFFF> Right click on the Jar to return Souls";
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
                InteractionHint1.text = "<color=red> This is a Key Item!";
                FadeInText(InteractionHint1);
                break;
            case HintForInteraction.MOUSETRAP:
                InteractionHint1.text = "<color=#9C00FF> Require Special Object to Activate Trap!";
                FadeInText(InteractionHint1);
                break;
            case HintForInteraction.RETURNSOULS:
                InteractionHint1.text = "<color=#FFFFFF> <size=60> Quickly Return them back to Soul Jar in the Office!";
                FadeInText(InteractionHint1);
                break;
            case HintForInteraction.FINISH:
                InteractionHint1.text = "<color=#FFFFFF> <size=60> Parents are back, wait them at Front Door";
                FadeInText(InteractionHint1);
                break;
            case HintForInteraction.JAR:
                InteractionHint2.text = "<color=#FFFFFF> Return ALL Souls back here.";
                FadeInText(InteractionHint2);
                break;
            case HintForInteraction.CROUCH:
                InteractionHint2.text = "<color=#FFFFFF> Left Shift Key to Crouch";
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

    void OnFallDeath(bool fall)
    {
        if (fall == true)
            deadPanel.GetComponentInChildren<TextMeshProUGUI>().text = "You Have Fell \n Return to Last Check Point";
        else
            deadPanel.GetComponentInChildren<TextMeshProUGUI>().text = "You are in Danger \n Return to Last Check Point";
    }

    void OnTimeChange(bool changedTime, float time)
    {
        if (changedTime)
        {
            timeChangeMessage.text = "<color=#FFC000> <size=80> +" + _GAME.rewardTime.ToString();
            timeChangeMessage.rectTransform.localPosition = new Vector3(0, 0f, 0);
        }
        else
        {
            timeChangeMessage.text = "<color=red> <size=80> -" + _GAME.punishmentTime.ToString();
            timeChangeMessage.rectTransform.localPosition = new Vector3(0, -30f, 0);
        }
        timeChangeMessage.alpha = 1;

        StartCoroutine(TimeChange(time));
    }

    public IEnumerator TimeChange (float time) //show time rewards/punishment
    {
        FadeInPanel(timeChangePanel);
        timeChangeMessage.rectTransform.DOLocalMoveY(50f, time);
        timeChangeMessage.DOFade(0, time + 0.5f);

        yield return new WaitForSeconds(time);
        FadeOutPanel(timeChangePanel);
    }
}

