using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class GameManager : Singleton<GameManager>
{
    //public static GameManager Instance;

    public Texture2D cursor;

    public PlayerMovement characterControl;
    // character rigidbody reference
    public float playerMass = 1f;
    [HideInInspector] //game states
    public bool dead = false, isPaused = false, gameOver = false, wonGame = false, pausePanel = false, menuPanel = false, optionPanel = false, playerActive = true;
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
        //if (Instance == null)
        //{
        //    Instance = this;
        //    DontDestroyOnLoad(gameObject);
        //}
        //else
        //    Destroy(this);

        if(cursor != null)
            Cursor.SetCursor(cursor, Vector2.zero, CursorMode.ForceSoftware);

        checkPoints.Add(characterControl.transform.position);
    }

    // Start is called before the first frame update
    void Start()
    {
        Restart();
    }

    public void Restart()
    {
        _PLAYER.Restart();
        pausePanel = false;
        menuPanel = false;
        dead = false;
        isPaused = false;
        gameOver = false;
        wonGame = false;
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
        //preventing errors when restart game.
        //if (main == null || second == null)
        //{
        //    main = Camera.main;
        //    foreach(Camera cam in Camera.allCameras)
        //    {
        //        if (cam.tag == "SecondCam")
        //            second = cam;
        //    }
        //}

        if (characterControl == null)
        {
            characterControl = FindObjectOfType<PlayerMovement>();
        }

        if (dead)
        {
            _timer -= punishmentTime;
            //SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            characterControl.transform.position = checkPoints[checkPoints.Count - 1];
            dead = false;
        }

        if (bottomReset != null)
        {
            if (characterControl.transform.position.y < bottomReset.position.y)
            {
                characterControl.transform.position = bottomReset.position;
                characterControl.fallDist = 0;
            }
        }

        if (!isPaused && _timer > 0) //timer count down and return energy when game is not paused.
        {
            _timer -= Time.deltaTime;

            if (_energy < maxEnergy)
            {
                _energy += energyReturnFactor * Time.deltaTime;
            }
        }

        if (isPaused)
            playerActive = false;
        else
        {
            if(playerActive == false)
            StartCoroutine("SetPlayerActive");
        }

        if (_timer <= 0)
        {
            gameOver = true;
            _timer = 0;
        }

        if (_energy < 0)
        {
            _energy = 0;
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (menuPanel)
                menuPanel = false;
            else
            {
                isPaused = !isPaused;
                pausePanel = !pausePanel;
            }
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

        //if (Input.GetKeyDown(KeyCode.V) && !isViewingAll)
        //{
        //    isViewingAll = true;
        //}
        //if (Input.GetKeyUp(KeyCode.V) && isViewingAll)
        //{
        //    isViewingAll = false;
        //}
        //if (isViewingAll)
        //{
        //    main.gameObject.SetActive(false);
        //    second.gameObject.SetActive(true);
        //}
        //if (!isViewingAll)
        //{
        //    main.gameObject.SetActive(true);
        //    second.gameObject.SetActive(false);
        //}


        //if(Input.GetKeyDown(KeyCode.P))
        //{
        //    isViewingAll = !isViewingAll;

        //    if(isViewingAll)
        //    {
        //        main.gameObject.transform.DOMove(zoomOutPos, 1);
        //    }
        //    else
        //    {
        //        main.gameObject.transform.DOMove(Vector3.zero, 1);
        //    }
        //}


        if (checkPoints.Count > 5)
            checkPoints.Remove(checkPoints[0]);
    }

    private void FixedUpdate()
    {
        
    }

    IEnumerator SetPlayerActive()
    {
        yield return new WaitForSeconds(0.1f);
        playerActive = true;
    }
}