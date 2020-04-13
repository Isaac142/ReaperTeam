using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public bool dead = false;
    public bool isHolding = false;
    public bool scytheEquiped = true;

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
        
    }

    IEnumerator Restart()
    {
        yield return new WaitForSeconds(1);

    }
}
