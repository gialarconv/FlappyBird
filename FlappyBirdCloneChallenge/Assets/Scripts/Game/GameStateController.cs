using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// this is a state machine, each event represent a different state of the game.
/// </summary>
public class GameStateController : MonoBehaviour
{
    public static System.Action OnPlayGame;
    public static System.Action OnPauseGame;
    public static System.Action OnGameOver;
    public static System.Action OnResetGame;
    public static Func<EnumGameState> OnGetCurrentGameState;

    [SerializeField] private EnumGameState _gameState;

    public EnumGameState GameState => _gameState;

    private void Awake()
    {
        OnGetCurrentGameState += GetCurrentGameState;
    }
    private void OnEnable()
    {
        OnPlayGame += PlayGame;
        OnPauseGame += PauseGame;
        OnGameOver += GameOver;
        OnResetGame += ResetGame;
    }

    private void OnDisable()
    {
        OnPlayGame -= PlayGame;
        OnPauseGame -= PauseGame;
        OnGameOver -= GameOver;
        OnResetGame -= ResetGame;
    }

    private EnumGameState GetCurrentGameState()
    {
        return _gameState;
    }
    private void PauseGame()
    {
        _gameState = EnumGameState.Paused;
    }
    private void PlayGame()
    {
        _gameState = EnumGameState.Progress;
    }
    private void ResetGame()
    {
        _gameState = EnumGameState.Menu;
    }
    private void GameOver()
    {
        _gameState = EnumGameState.GameOver;
    }
}