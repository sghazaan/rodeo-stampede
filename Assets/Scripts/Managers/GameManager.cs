using UnityEngine;

public static class GameManager
{
    public static bool isPlayerRiding = false;
    public enum GameState { Idle, Playing, GameOver }

    #region Public Getters and Setters
   
    public static bool IsPlayerRiding
    {
        get => isPlayerRiding;
        set => isPlayerRiding = value;
    }

    public static GameState CurrentState { get;  set; } = GameState.Idle;
    public static bool IsGameOver => CurrentState == GameState.GameOver;
    public static bool IsPlaying => CurrentState == GameState.Playing;

    #endregion
}