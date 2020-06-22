using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UpdateUI : Singleton<UpdateUI>
{
    [Header ("In Game UI")]
    public GameObject UIs;  //in game UI display (timer, scythe icons and souls)
    public Text timerCount;
    public Image timer;

    public List<Image> souls = new List<Image>();
    public List<Image> soulMasks = new List<Image>();
    public Text totalSoulNo;
    public GameObject infoPanel;
    public Text itemName, itemType, itemDescription;
    public GameObject menu;
    public Slider energyBar;
    public Image abilityCD;
    public GameObject[] masks;  //scythe icon masks

    [Header ("GameStatePanels")]
    public GameObject pausePanel;
    public GameObject gameOverPanel;
    public GameObject instrunctionPanel;
    public GameObject controlsPanel;
    public GameObject uiPanel;
    public GameObject optionPanel;
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

        if(_GAME.scytheEquiped)
        {
            foreach (GameObject pic in masks)
                pic.SetActive(false);
        }
        else
        {
            foreach (GameObject pic in masks)
                pic.SetActive(true);
        }

        abilityCD.fillAmount = _GAME.CDTimer / _GAME.coolDown;

        totalSoulNo.text = _GAME.totalSoulNo.ToString();
    }
    public void CloseAllPanels()
    {
        pausePanel.SetActive(false);
        gameOverPanel.SetActive(false);
        menu.SetActive(false);
        wonPanel.SetActive(false);
    }
}
