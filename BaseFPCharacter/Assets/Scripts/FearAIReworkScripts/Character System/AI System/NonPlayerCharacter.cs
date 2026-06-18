using System;
using UnityEngine;

public class NonPlayerCharacter : CharacterGeneral
{
    [SerializeField]
    private int[] pointsOnElimination;

    public static Action<int> onKilled;

    private string whereWasFinalShot, fromWhoWasFinalShot;

    public void WhatKilledThem(string whereWasFinalShot, string fromWhoWasFinalShot)
    {
        this.whereWasFinalShot = whereWasFinalShot;
        this.fromWhoWasFinalShot = fromWhoWasFinalShot;
    }

    private void OnDestroy()
    {
        if (fromWhoWasFinalShot != "Player") { return; }

        int finalSetOfPoints = 0;

        switch (whereWasFinalShot)
        {
            case "MainBody":
                finalSetOfPoints = pointsOnElimination[0];
                break;
            case "Head":
                finalSetOfPoints = pointsOnElimination[1];
                break;
            default:
                finalSetOfPoints = pointsOnElimination[2];
                break;
        }
        onKilled?.Invoke(finalSetOfPoints);
    }
}
