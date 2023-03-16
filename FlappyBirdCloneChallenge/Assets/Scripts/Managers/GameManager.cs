using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static GameManager _instance;
    public static GameManager Instance => _instance;

    [Header("Game modifiers")]
    [SerializeField] private float _gameSpeed = 5f;
    [SerializeField] private float _timeToResumeGame = 3f;
    [Header("Scripts References")]
    [SerializeField] private GameScoreController _gameScoreController;
    [SerializeField] private ChunksController _chunksController;

    private WaitForSeconds _waitToResumeGame;

    public float GameSpeed => _gameSpeed;
    public int CurrentScore => _gameScoreController.GetCurrentScore();
    public float TimeToResumeGame => _timeToResumeGame;
    public WaitForSeconds WaitToResumeGame => _waitToResumeGame;

    [InspectorButton("Reset Game", nameof(ResetGame))]
    public bool _resetGameButton;

    private void Awake()
    {
        _instance = this;
    }
    private void Start()
    {
        _gameScoreController.InitGameScore();
        _chunksController.InitChunks();

        _waitToResumeGame = new WaitForSeconds(_timeToResumeGame);
    }

    public void ResetGame()
    {
        _gameScoreController.InitGameScore();
        _chunksController.InitChunks();
    }
    public void GamePaused(float delay = 0f)
    {
        StopAllCoroutines();
        GameStateController.OnPauseGame?.Invoke();
        StartCoroutine(PauseGameTime(delay));
    }
    public void GameResume(System.Action OnAction)
    {
        StopCoroutinesTime();
        StartCoroutine(ResumeGameTime(OnAction));
    }
    public void StopCoroutinesTime()
    {
        StopAllCoroutines();
        Time.timeScale = 1f;
    }
    private IEnumerator PauseGameTime(float delay)//default 2f
    {
        float pauseTime = Time.time + delay;
        float decrement = (delay > 0) ? Time.deltaTime / delay : Time.deltaTime;

        while (Time.timeScale > 0.1f || Time.time < pauseTime)
        {
            Time.timeScale = Mathf.Clamp(Time.timeScale - decrement, 0f, Time.timeScale - decrement);
            yield return null;
        }

        // ramp the timeScale down to 0
        Time.timeScale = 0f;
    }
    private IEnumerator ResumeGameTime(System.Action OnAction)
    {
        yield return _waitToResumeGame;
        OnAction?.Invoke();
        GameStateController.OnPlayGame?.Invoke();
    }
}