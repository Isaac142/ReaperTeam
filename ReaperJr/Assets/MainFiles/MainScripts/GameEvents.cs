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
    public static event Action<HintForMovingBoxes> OnMovableHintShown = null;
    public static event Action<HintForInteraction> OnInteractHintShown = null;
    public static event Action<HintForItemCollect> OnCollectHintShown = null;
    public static event Action<KeyItem> OnKeyItemCollected = null;
    public static event Action<bool> OnCrossHairOut = null;
    public static event Action<EnemyPatrol> OnFakeSoulChasing = null;
    public static event Action<FakeSoulController> OnFakeSoulCollected = null;
    public static event Action<bool> OnMovingObject = null;

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

    public static void ReportMovableHintShown(HintForMovingBoxes action)
    {
        if (OnMovableHintShown != null)
            OnMovableHintShown(action);
    }

    public static void ReportKeyItemCollected(KeyItem itemCollected)
    {
        if (OnKeyItemCollected != null)
            OnKeyItemCollected(itemCollected);
    }

    public static void ReportCollectHintShown(HintForItemCollect action)
    {
        if (OnCollectHintShown != null)
            OnCollectHintShown(action);
    }

    public static void ReportInteractHintShown(HintForInteraction action)
    {
        if (OnInteractHintShown != null)
            OnInteractHintShown(action);
    }

    public static void RepoartCrossHairOut(bool crosshair)
    {
        if (OnCrossHairOut != null)
            OnCrossHairOut(crosshair);
    }

    public static void ReportOnFakeSoulChasing(EnemyPatrol fakeSoul)
    {
        if (OnFakeSoulChasing != null)
            OnFakeSoulChasing(fakeSoul);
    }

    public static void ReportOnFakeSoulCollected(FakeSoulController fakeSoul)
    {
        if (OnFakeSoulChasing != null)
            OnFakeSoulCollected(fakeSoul);
    }

    public static void ReportOnMovingObject(bool holding)
    {
        if (OnMovingObject != null)
            OnMovingObject(holding);
    }
}
