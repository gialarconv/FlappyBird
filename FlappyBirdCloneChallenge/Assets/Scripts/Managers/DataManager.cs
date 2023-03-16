using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataManager : MonoBehaviour
{
    public static System.Action<int> OnSaveData;
    public static Func<int> OnGetScore;

    [SerializeField] private string _fileName = "GameData";
    [SerializeField] private int _testScore;

    private string _fullPath;
    //if true data will be encripted with an XOR function
    private bool encrypt = false;
    private GameData _gameData;

    [InspectorButton("Save Data", nameof(SaveTest))]
    public bool _saveButton;
    [InspectorButton("Load Data", nameof(LoadGameValues))]
    public bool _loadButton;
    [InspectorButton("Clear Data", nameof(ClearData))]
    public bool _clearButton;

    private void Awake()
    {
        //the filename where saved data will be stored
        _fullPath = Application.persistentDataPath + "/" + _fileName;
        _gameData = new GameData();

        OnGetScore += GetScore;
    }
    private void OnEnable()
    {
        OnSaveData += SaveGameValues;
    }
    private void OnDisable()
    {
        OnSaveData -= SaveGameValues;
    }
    void Start()
    {
        LoadGameValues();
    }

    public void LoadGameValues()
    {
        SaveManager.Instance.Load<GameData>(_fullPath, DataWasLoaded, encrypt);
    }
    private void DataWasLoaded(GameData data, SaveResult result, string message)
    {
        if (result == SaveResult.EmptyData || result == SaveResult.Error)
        {
            data = new GameData();
        }
        if (result == SaveResult.Success)
        {
            _gameData = data;

            _testScore = _gameData.gameScore;
        }
    }
    public void SaveGameValues(int score)
    {
        _gameData.gameScore = score;

        SaveManager.Instance.Save(_gameData, _fullPath, DataWasSaved, encrypt);
    }
    private void DataWasSaved(SaveResult result, string message)
    {
        Debug.Log($"Data Was Saved - Result: {result}, message {message}");
        if (result == SaveResult.Error)
        {
            Debug.Log($"Error saving data");
        }
    }

    public int GetScore()
    {
        return _gameData.gameScore;
    }

    public void SaveTest()
    {
        SaveGameValues(_testScore);
    }
    public void ClearData()
    {
        Debug.Log($"Data was clear");
        SaveManager.Instance.ClearFIle(_fullPath);
    }
}