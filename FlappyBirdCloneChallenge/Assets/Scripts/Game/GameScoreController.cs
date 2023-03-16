using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameScoreController : MonoBehaviour
{
    public static System.Action OnAddScore;

    [SerializeField] private int _scoreToAdd;

    private int _gameCurrentScore;

    private void OnEnable()
    {
        GameStateController.OnResetGame += InitGameScore;
        GameStateController.OnGameOver += GameOver;
        OnAddScore += AddScore;
    }

    private void OnDisable()
    {
        GameStateController.OnResetGame -= InitGameScore;
        GameStateController.OnGameOver -= GameOver;
        OnAddScore -= AddScore;
    }

    public void InitGameScore()
    {
        _gameCurrentScore = 0;
    }

    private void AddScore()
    {
        _gameCurrentScore += _scoreToAdd;
        ScreenController.OnReplaceCurrentGameScore?.Invoke(_gameCurrentScore);
    }

    public int GetCurrentScore()
    {
        return _gameCurrentScore;
    }

    private void GameOver()
    {
        if (DataManager.OnGetScore() < _gameCurrentScore)
        {
            //save data first
            DataManager.OnSaveData?.Invoke(_gameCurrentScore);
            //replace the new data into menu screen
            ScreenController.OnReplaceMenuGameScoreScreen?.Invoke();
        }
    }
}