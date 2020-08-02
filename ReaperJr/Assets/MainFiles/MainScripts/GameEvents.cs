using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public static class GameEvents
{
    public static event Action<GameState> OnGameStateChange = null;
    public static event Action<bool> OnScytheEquipped = null;
    public static event Action<SoulType> OnSoulCollected = null;
    public static event Action<bool> OnScytheThrow = null;
    public static event Action<HintForActions> OnHintShown = null;
    public static event Action<KeyItem> OnKeyItemCollected = null;
    public static event Action<bool> OnCrossHairOut = null;

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

    public static void ReportScytheThrown(bool ScytheThrown)
    {
        if (OnScytheThrow != null)
            OnScytheThrow(ScytheThrown);
    }

    public static void ReportHintShown(HintForActions action)
    {
        if (OnHintShown != null)
            OnHintShown(action);
    }

    public static void ReportKeyItemCollected(KeyItem itemCollected)
    {
        if (OnKeyItemCollected != null)
            OnKeyItemCollected(itemCollected);
    }

    public static void RepoartCrossHairOut(bool crosshair)
    {
        if (OnCrossHairOut != null)
            OnCrossHairOut(crosshair);
    }
}
