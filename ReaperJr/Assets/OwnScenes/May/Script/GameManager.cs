using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public bool dead = false;
    public bool isHolding = false;
    public bool holdingLightObject = false;
    public bool scytheEquiped = true;
    public bool isTeleported = false;
    public bool onSpecialGround = false;

    public float maxTimerInSeconds = 5 * 60f;
    public float warningTimeInSeconds =  60f;
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

    public bool isPaused = false;
    public bool gameOver = false;


    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
            Destroy(this);
    }

        // Start is called before the first frame update
        void Start()
    {
        dead = false;
        isPaused = false;
        gameOver = false;
        holdingLightObject = false;
        isHolding = false;
        onSpecialGround = false;
        _timer = maxTimerInSeconds;
        _energy = maxEnergy;
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

        if(_energy < 0)
        {
            _energy = 0;
        } 
        
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            isPaused = !isPaused;
        }
    }
}
