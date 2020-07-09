using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Buttons : ReaperJr
{
    private void OnMouseOver()
    {
        _GAME.playerActive = false;
    }
    private void OnMouseExit()
    {
        _GAME.playerActive = true;
    }
}
