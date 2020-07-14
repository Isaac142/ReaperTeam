using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public static class GameEvents
{
    public static event Action<GameState> OnGameStateChange = null;
    public static event Action<bool> OnScytheEquipped = null;
    public static event Action<SoulType> OnSoulCollected = null;

    public static void ReportGameStateChange(GameState gameState)
    {
        if (OnGameStateChange != null)
            OnGameStateChange(gameState);
    }

    public static void ReportScytheEquipped(bool scytheEquipped)
    {
        if (OnScytheEquipped != null)
            OnScytheEquipped(scytheEquipped);
    }

    public static void ReportSoulCollected(SoulType soulCollected)
    {
        if (OnSoulCollected != null)
            OnSoulCollected(soulCollected);
    }
}
