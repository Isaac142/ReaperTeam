﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class GameManager : Singleton<GameManager>
{
    public enum GameState { TITLE, INGAME, PAUSED, DEAD, RESUME, GAMEOVER, WON, MENU}
    public GameState gameState;
    private float lastStateChange = 0f;
    //public static GameManager Instance;

    public Texture2D cursor;
    
    // character rigidbody reference
    public float playerMass = 1f;
    [HideInInspector] //game states
    public bool playerActive = true, isPaused =  false;
    [HideInInspector] //holding object states
    public bool isHolding = false, canHold = true, holdingLightObject = false;
    [HideInInspector] //scythe and its ability state
    public bool scytheEquiped = true, onCD = false;
    [HideInInspector] //grounding states
    public bool onSpecialGround = false;
    private bool pauseCheck;

    public float maxSafeFallDist = 8f;

    public float maxTimerInSeconds = 5 * 60f;
    public float warningTimeInSeconds = 60f;
    public float rewardTime = 10f;
    public float punishmentTime = 5f;
    private float _timer;
    public float Timer
    {
        get { return _timer; }
        set { _timer = value; }
    }

    public float maxEnergy = 100f;
    public float throwEngery = 5f;
    public float teleportingEnergy = 20f;
    public float energyReturnFactor = 1f;
    private float _energy;
    public float Energy
    {
        get { return _energy; }
        set { _energy = value; }
    }

    public float coolDown = 5f;

    private float _cDTimer;
    public float CDTimer
    {
        get { return _cDTimer; }
        set { _cDTimer = value; }
    }

    [HideInInspector]
    public List<Vector3> checkPoints = new List<Vector3>();
    public int totalSoulNo = 0;
    public Transform bottomReset;

    private void Awake()
    {
        if(cursor != null)
            Cursor.SetCursor(cursor, Vector2.zero, CursorMode.ForceSoftware);

        checkPoints.Add(_PLAYER.transform.position);
    }

    // Start is called before the first frame update
    void Start()
    {
        Restart();
        SetGameState(GameState.INGAME);
    }

    public void Restart()
    {
        _PLAYER.Restart();
        holdingLightObject = false;
        isHolding = false;
        canHold = true;
        onSpecialGround = false;
        _timer = maxTimerInSeconds;
        _energy = maxEnergy;
        _cDTimer = coolDown;
        onCD = false;
        _GAME.totalSoulNo = 0;
        Time.timeScale = 1;
    }

    // Update is called once per frame
    void Update()
    {
        switch(gameState)
        {
            case GameState.INGAME:

                _timer -= Time.deltaTime; //Count down timer.

                if (_energy < maxEnergy)  //energy recovery
                {
                    _energy += energyReturnFactor * Time.deltaTime;
                }

                if (_energy < 0)
                {
                    _energy = 0;
                }

                if (onCD) 
                {
                    _cDTimer += Time.deltaTime;
                }

                if (_cDTimer >= coolDown)
                {
                    onCD = false;
                    _cDTimer = coolDown;
                }

                if (Input.GetKeyDown(KeyCode.Escape))
                    SetGameState(GameState.PAUSED);

                if (_timer <= 0)
                {
                    SetGameState(GameState.GAMEOVER);
                    _timer = 0;
                }
                break;

            case GameState.DEAD:
                _timer -= punishmentTime;
                _PLAYER.transform.position = checkPoints[checkPoints.Count - 1];
                SetGameState(GameState.INGAME);
                break;

            case GameState.PAUSED:
                PauseGame();
                _UI.pausePanel.SetActive(true);

                if (Input.GetKeyDown(KeyCode.Escape))
                    SetGameState(GameState.RESUME);
                break;

            case GameState.MENU:
                PauseGame();
                _UI.menuPanel.SetActive(true);
                if (Input.GetKeyDown(KeyCode.Escape))
                    SetGameState(GameState.RESUME);
                break;

            case GameState.RESUME:
                Time.timeScale = 1;
                isPaused = false;
                _UI.CloseAllPanels();
                if (Time.time - lastStateChange >= 0.1)
                {
                    playerActive = true;
                    SetGameState(GameState.INGAME);
                }
                break;

            case GameState.GAMEOVER:
                PauseGame();
                _UI.gameOverPanel.SetActive(true);
                break;

            case GameState.WON:
                PauseGame();
                _UI.wonPanel.SetActive(true);
                break;
        }

        //preventing player fall of the world
        if (bottomReset != null)
        {
            if (_PLAYER.transform.position.y < bottomReset.position.y)
            {
                _PLAYER.transform.position = bottomReset.position;
                _PLAYER.fallDist = 0;
            }
        }

        if (checkPoints.Count > 5) // delete check points record to keep list small
            checkPoints.Remove(checkPoints[0]);
    }

    public void SetGameState(GameState state)
    {
        gameState = state;
        lastStateChange = Time.time;
    }

    void PauseGame()
    {
        Time.timeScale = 0;
        playerActive = false;
        isPaused = true;
    }
}