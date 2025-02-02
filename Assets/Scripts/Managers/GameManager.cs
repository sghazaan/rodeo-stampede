using UnityEngine;

public static class GameManager
{
    public static bool isGameLost = false;
    public static bool isGameWon = false;
    public static bool isPlayerRiding = false;

    #region Public Getters and Setters
    public static bool IsGameLost
    {
        get => isGameLost;
        set => isGameLost = value;
    }

    public static bool IsGameWon
    {
        get => isGameWon;
        set => isGameWon = value;
    }

    public static bool IsPlayerRiding
    {
        get => isPlayerRiding;
        set => isPlayerRiding = value;
    }
    #endregion
}