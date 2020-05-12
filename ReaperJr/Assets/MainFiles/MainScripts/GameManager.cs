using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public Texture2D cursor;
    public Camera main;
    public Camera second;
    public bool isViewingAll;

    public PlayerMovement characterControl;
    // character rigidbody reference
    public float playerMass = 1f;
    [HideInInspector] //game states
    public bool dead = false, isPaused = false, gameOver = false;
    [HideInInspector] //holding object states
    public bool isHolding = false, canHold = true, holdingLightObject = false;
    [HideInInspector] //scythe and its ability state
    public bool scytheEquiped = true, onCD = false;
    [HideInInspector] //grounding states
    public bool onSpecialGround = false;

    public float maxSafeFallDist = 5f;

    public float maxTimerInSeconds = 5 * 60f;
    public float warningTimeInSeconds = 60f;
    public float rewardTime = 10f;
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


    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
            Destroy(this);

        if(cursor != null)
            Cursor.SetCursor(cursor, Vector2.zero, CursorMode.ForceSoftware);
    }

    // Start is called before the first frame update
    void Start()
    {
        dead = false;
        isPaused = false;
        gameOver = false;
        holdingLightObject = false;
        isHolding = false;
        canHold = true;
        onSpecialGround = false;
        _timer = maxTimerInSeconds;
        _energy = maxEnergy;
        _cDTimer = coolDown;
        onCD = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (dead)
        {
            //SceneManager.LoadScene(0);
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            dead = false;
        }

        if (!isPaused && _timer > 0) //timer count down and return energy when game is not paused.
        {
            _timer -= Time.deltaTime;

            if (_energy < maxEnergy)
            {
                _energy += energyReturnFactor * Time.deltaTime;
            }
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
            isPaused = !isPaused;
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

        if (Input.GetKeyDown(KeyCode.V) && !isViewingAll)
        {
            isViewingAll = true;
        }
        if (Input.GetKeyUp(KeyCode.V) && isViewingAll)
        {
            isViewingAll = false;
        }
        if (isViewingAll)
        {
            main.gameObject.SetActive(false);
            second.gameObject.SetActive(true);
        }
        if (!isViewingAll)
        {
            main.gameObject.SetActive(true);
            second.gameObject.SetActive(false);
        }
    }
}