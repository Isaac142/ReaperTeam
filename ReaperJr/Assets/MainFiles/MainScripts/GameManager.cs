using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;
public enum GameState { TITLE, OPENNING, INGAME, PAUSED, DEAD, RESUME, GAMEOVER, WON, MENU, VICTORY }

public class GameManager : Singleton<GameManager>
{
    
    public GameState gameState;
    private float lastStateChange = 0f;
    
    // character rigidbody reference
    public float playerMass = 1f;
    public GameObject deadParticleEffect;
    [HideInInspector] //game states
    public bool playerActive = true, isPaused =  false;
    [HideInInspector] //holding object states
    public bool isHolding = false, holdingLightObject = false;
    [HideInInspector] //scythe and its ability state
    public bool scytheEquiped = true, scytheaThrown = false, onCD = false;
    [HideInInspector] //grounding states
    public bool onSpecialGround = false;

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
    [HideInInspector]
    public bool isInvincible = false;
    public float invincibleTime = 5f;

    // Start is called before the first frame update
    void Start()
    {
        GameEvents.ReportGameStateChange(GameState.TITLE);

        if (bottomReset != null)
            DontDestroyOnLoad(bottomReset.gameObject);
    }

    public void ResetGame()
    {
        checkPoints = new List<Vector3>();
        _UI.StartSetUI();
        _PLAYER.Restart();
        holdingLightObject = false;
        isHolding = false;
        onSpecialGround = false;
        _timer = maxTimerInSeconds;
        _energy = maxEnergy;
        _cDTimer = coolDown;
        onCD = false;
        _GAME.totalSoulNo = 0;
        //Time.timeScale = 1;
    }

    // Update is called once per frame
    void Update()
    {
        switch(gameState)
        {
            case GameState.INGAME:
                if (Input.GetKeyDown(KeyCode.Escape))
                    GameEvents.ReportGameStateChange(GameState.PAUSED);

                _timer -= Time.deltaTime; //Count down timer.

                _energy += energyReturnFactor * Time.deltaTime;
                _energy = Mathf.Clamp(_energy, 0f, maxEnergy);

                if (onCD) 
                {
                    _cDTimer += Time.deltaTime;
                }

                if (_cDTimer >= coolDown)
                {
                    onCD = false;
                    _cDTimer = coolDown;
                }

                if(_energy < teleportingEnergy + throwEngery)
                {
                    onCD = true;
                }
               
                if (_timer <= 0)
                {
                    GameEvents.ReportGameStateChange(GameState.GAMEOVER);
                    _timer = 0;
                }
                break;

            case GameState.PAUSED:
                if (Input.GetKeyDown(KeyCode.Escape))
                    GameEvents.ReportGameStateChange(GameState.RESUME);
                break;

            case GameState.MENU:
                if (Input.GetKeyDown(KeyCode.Escape))
                    GameEvents.ReportGameStateChange(GameState.RESUME);
                break;
            case GameState.VICTORY:
                if (Input.GetKeyDown(KeyCode.Escape))
                    GameEvents.ReportGameStateChange(GameState.RESUME);
                break;
            case GameState.OPENNING:
                if (Input.GetKeyDown(KeyCode.Escape))
                    GameEvents.ReportGameStateChange(GameState.RESUME);
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

    void PlayerDead()
    {
        _PLAYER.teleportStart.SetActive(false);
        _PLAYER.teleportEnd.SetActive(false);
        _cDTimer = coolDown;
        onCD = false;
        _timer -= punishmentTime;
        playerActive = false;
        _UI.SetHintPanel();
        //_PLAYER.transform.position = checkPoints[checkPoints.Count - 1];
        
        if(checkPoints.Count <= 0) // return to starting point if no saved check point.
        {
            _PLAYER.Restart();
            StartCoroutine(DeadtoInGame());
        }
        else
            _PLAYER.transform.DOMove(checkPoints[checkPoints.Count - 1], 3)
            .OnComplete(() =>
             GameEvents.ReportGameStateChange(GameState.RESUME));
 
        deadParticleEffect.SetActive(true);
        StartCoroutine(InvincibleTimer());
    }

    public void OnGameStateChange(GameState state)
    {
        gameState = state;
        switch (state)
        {
            case GameState.TITLE:
                _AUDIO.PlayMusic("Theme");
                PauseGame();
                break;
            case GameState.OPENNING:
                _AUDIO.PlayMusic("Theme");
                PauseGame();
                break;
            case GameState.INGAME:
                deadParticleEffect.SetActive(false);
                GameEvents.ReportOnFallDeath(false);
                _AUDIO.PlayMusic("Theme");
                break;
            case GameState.DEAD:
                if (isInvincible) //attempt to stop repeat dead-in game state change.
                    GameEvents.ReportGameStateChange(GameState.INGAME);
                else
                {
                    PlayerDead();
                    _AUDIO.PlayMusic("PlayerReset");
                }
                break;
            case GameState.PAUSED:
                _AUDIO.PlayMusic("Theme");
                PauseGame();
                break;
            case GameState.MENU:
                _AUDIO.PlayMusic("Theme");
                PauseGame();
                break;
            case GameState.GAMEOVER:
                _AUDIO.PlayMusic("Theme");
                PauseGame();
                break;
            case GameState.WON:
                _AUDIO.PlayMusic("Theme");
                PauseGame();
                break;
            case GameState.RESUME:
                StartCoroutine(ResumeGame());
                break;
            case GameState.VICTORY:
                _PLAYER.transform.eulerAngles = new Vector3(0, 90, 0);
                _PLAYER.anim.SetTrigger("Victory");
                break;
        }
    }

    public void PauseGame()
    {
        //Time.timeScale = 0;
        playerActive = false;
        isPaused = true;
    }

    IEnumerator ResumeGame()
    {
        //Time.timeScale = 1;
        yield return new WaitForSeconds(0.5f);

        isPaused = false;
        playerActive = true;
        GameEvents.ReportGameStateChange(GameState.INGAME);
    }

    void OnScytheEquipped(bool scythe)
    {
        scytheEquiped = scythe;
    }

    void OnScytheThrow(bool scythe)
    {
        scytheaThrown = scythe;
    }

    private void OnEnable()
    {
        GameEvents.OnGameStateChange += OnGameStateChange;
        GameEvents.OnScytheEquipped += OnScytheEquipped;
        GameEvents.OnScytheThrow += OnScytheThrow;
    }

    private void OnDisable()
    {
        GameEvents.OnGameStateChange -= OnGameStateChange;
        GameEvents.OnScytheEquipped -= OnScytheEquipped;
        GameEvents.OnScytheThrow -= OnScytheThrow;
    }

    IEnumerator InvincibleTimer() //invincible after respawn
    {
        isInvincible = true;
        yield return new WaitForSeconds(invincibleTime);
        isInvincible = false;
    }

    IEnumerator DeadtoInGame()
    {
        yield return new WaitForSeconds(3f);
        GameEvents.ReportGameStateChange(GameState.RESUME);
    }
}