using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Buttons : MonoBehaviour
{

    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        GameManager.Instance.isPaused = false;
        GameManager.Instance.Energy = GameManager.Instance.maxEnergy;
        GameManager.Instance.Timer = GameManager.Instance.maxTimerInSeconds;
        GameManager.Instance.dead = false;
        GameManager.Instance.gameOver = false;
        GameManager.Instance.holdingLightObject = false;
        GameManager.Instance.isHolding = false;
        GameManager.Instance.canHold = true;
        GameManager.Instance.onSpecialGround = false;
        Time.timeScale = 1;
    }

    public void Return()
    {
        GameManager.Instance.isPaused = false;
    }
}
