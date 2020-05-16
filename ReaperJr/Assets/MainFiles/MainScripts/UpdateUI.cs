using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UpdateUI : MonoBehaviour
{
    public GameObject UIs;
    public Text timerCount;
    public Image timer;

    public GameObject pausePanel;
    public GameObject gameOverPanel;
    public GameObject menu;
    public GameObject instrunctionPanel;
    public GameObject controlsPanel;
    public GameObject uiPanel;
    public GameObject optionPanel;
    public GameObject wonPanel;
    public List<Image> souls = new List<Image>();
    public List<Image> soulMasks = new List<Image>();
    public Text totalSoulNo;
    public GameObject infoPanel;
    public Text itemName, itemType, itemDescription;

    public Slider energyBar;
    public Image abilityCD;
    public GameObject[] masks;
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
        energyBar.maxValue = GameManager.Instance.maxEnergy;
        energyBar.minValue = 0f;

        pausePanel.SetActive(false);
        gameOverPanel.SetActive(false);
        menu.SetActive(false);

        foreach (GameObject pic in masks)
            pic.SetActive(false);

        infoPanel.SetActive(false);   
    }

    // Update is called once per frame
    void Update()
    {
        if(GameManager.Instance.Timer < GameManager.Instance.warningTimeInSeconds)
        {
            timerCount.text = FormatTimeSSMilS(GameManager.Instance.Timer);
            timerCount.GetComponent<Text>().color = Color.yellow;
            timerCount.fontSize = 45;
        }
        else
            timerCount.text = FormatTimeMMSS(GameManager.Instance.Timer);
        if(GameManager.Instance.Timer/GameManager.Instance.maxTimerInSeconds < GameManager.Instance.warningTimeInSeconds / GameManager.Instance.maxTimerInSeconds)
        {
            timer.fillAmount = timer.fillAmount = GameManager.Instance.Timer / GameManager.Instance.maxTimerInSeconds;
            timer.GetComponent<Image>().color = Color.red;
        }
        else
        timer.fillAmount = GameManager.Instance.Timer / GameManager.Instance.maxTimerInSeconds;

        energyBar.value = GameManager.Instance.Energy;

        if(GameManager.Instance.scytheEquiped)
        {
            foreach (GameObject pic in masks)
                pic.SetActive(false);
        }
        else
        {
            foreach (GameObject pic in masks)
                pic.SetActive(true);
        }

        abilityCD.fillAmount = GameManager.Instance.CDTimer / GameManager.Instance.coolDown;

        if (GameManager.Instance.pausePanel)
        {
            Time.timeScale = 0;
            pausePanel.SetActive(true);
            GameManager.Instance.playerActive = false;
        }
        else
        {
            pausePanel.SetActive(false);
            Time.timeScale = 1;
        }

        if (GameManager.Instance.gameOver)
            gameOverPanel.SetActive(true);
        else
            gameOverPanel.SetActive(false);

        if (GameManager.Instance.menuPanel)
        {
            menu.SetActive(true);
            UIs.SetActive(false);
            GameManager.Instance.isPaused = true;
        }
        else
        {
            UIs.SetActive(true);
            menu.SetActive(false);
            GameManager.Instance.isPaused = false;
        }

        if (GameManager.Instance.wonGame)
            wonPanel.SetActive(true);
        else
            wonPanel.SetActive(false);

        totalSoulNo.text = GameManager.Instance.totalSoulNo.ToString();
    }
}
