using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization.Settings;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.UIElements;

public class ScreenController : MonoBehaviour
{
    public static System.Action OnReplaceMenuGameScoreScreen;
    public static System.Action<int> OnReplaceCurrentGameScore;

    [SerializeField] protected UIDocument _uiDocument;
    [SerializeField] protected Volume _volume;

    private VisualElement _rootScreen;
    private ScreenItemComponent _screenItemComponent;
    private string _highestScoreLocalizationSubString;
    private string _currentScoreLocalizationString;
    private string _finalScoreLocalizationSubString;

    protected void Awake()
    {
        _rootScreen = _uiDocument.rootVisualElement;
    }
    private void OnEnable()
    {
        OnReplaceMenuGameScoreScreen += ReplaceMenuGameScoreScreen;
        OnReplaceCurrentGameScore += ReplaceCurrentGameScore;
        GameStateController.OnGameOver += GameOver;
    }

    private void OnDisable()
    {
        OnReplaceMenuGameScoreScreen -= ReplaceMenuGameScoreScreen;
        OnReplaceCurrentGameScore -= ReplaceCurrentGameScore;
        GameStateController.OnGameOver -= GameOver;
    }
    private IEnumerator Start()
    {
        yield return LocalizationSettings.InitializationOperation;

        _screenItemComponent = new ScreenItemComponent();
        _screenItemComponent.SetVisualElements(_rootScreen);

        GetScoreSubString();

        RegisterButtonCallbacks();

        InitScreens();

        ReplaceMenuGameScoreScreen();
    }
    /// <summary>
    /// Enable/disable all the screens in order to show the menu first
    /// </summary>
    private void InitScreens()
    {
        ShowVisualElement(_screenItemComponent.menuScreen, true);
        ShowVisualElement(_screenItemComponent.pauseScreen, false);
        ShowVisualElement(_screenItemComponent.timerScreen, false);
        ShowVisualElement(_screenItemComponent.gameScreen, false);
        ShowVisualElement(_screenItemComponent.gameOverScreen, false);
        ShowVisualElement(_screenItemComponent.quitGameScreen, false);
        ShowVisualElement(_screenItemComponent.abandonScreen, false);

        BlurBackground(true);

#if UNITY_STANDALONE_WIN || UNITY_EDITOR
        ShowVisualElement(_screenItemComponent.quitGameScreen, true);
#endif
    }
    #region Score
    //save the strings from Localization, since this label are dynamics
    private void GetScoreSubString()
    {
        //get current score text => Score:
        _currentScoreLocalizationString = _screenItemComponent.currentScoreText.text.Split('?')[0];
        //get highest score text => Highest score:
        _highestScoreLocalizationSubString = _screenItemComponent.gameHighestScoreText.text.Split('?')[0];
        //get final score text => Final score:
        _finalScoreLocalizationSubString = _screenItemComponent.finalScoreText.text.Split('?')[0];
    }
    private void ReplaceMenuGameScoreScreen()
    {
        ReplaceGameScoreScreen(_screenItemComponent.menuHighestScoreText, _highestScoreLocalizationSubString, DataManager.OnGetScore());
    }
    private void ReplaceCurrentGameScore(int amount)
    {
        ReplaceGameScoreScreen(_screenItemComponent.currentScoreText, _currentScoreLocalizationString, amount);
    }
    private void ReplaceGameScoreScreen(Label label, string text, int amount)
    {
        label.text = $"{text} {amount}";
    }
    #endregion

    #region Click Events
    //delegate the different actions to the different buttons
    private void RegisterButtonCallbacks()
    {
        _screenItemComponent.playButton?.RegisterCallback<ClickEvent>(PlayGame);
        _screenItemComponent.pauseButton?.RegisterCallback<ClickEvent>(ShowPauseScreen);
        _screenItemComponent.pauseQuitButton?.RegisterCallback<ClickEvent>(ReturnToMenu);
        _screenItemComponent.resumeButton?.RegisterCallback<ClickEvent>(HidePauseScreen);
        _screenItemComponent.gameOverQuitButton?.RegisterCallback<ClickEvent>(ShowMenuScreen);

        _screenItemComponent.quitGameButton?.RegisterCallback<ClickEvent>(QuitGame);
    }
    private void PlayGame(ClickEvent evt)
    {
        //disable the Global volume
        BlurBackground(false);

        //enable/disable the differents panels in the UXML
#if UNITY_STANDALONE_WIN || UNITY_EDITOR
        ShowVisualElement(_screenItemComponent.quitGameScreen, false);
#endif
        ShowVisualElement(_screenItemComponent.menuScreen, false);
        ShowVisualElement(_screenItemComponent.gameScreen, true);

        //change game state
        GameStateController.OnPlayGame?.Invoke();

        //replace the scores on screen
        ReplaceGameScoreScreen(_screenItemComponent.gameHighestScoreText, _highestScoreLocalizationSubString, DataManager.OnGetScore());
        ReplaceGameScoreScreen(_screenItemComponent.currentScoreText, _currentScoreLocalizationString, 0);
    }

    private void ShowPauseScreen(ClickEvent evt)
    {
        GameManager.Instance.GamePaused(.5f);

        ShowVisualElement(_screenItemComponent.gameScreen, false);
        ShowVisualElement(_screenItemComponent.pauseScreen, true);


        BlurBackground(true);
    }

    private void HidePauseScreen(ClickEvent evt)
    {
        ShowVisualElement(_screenItemComponent.gameScreen, true);
        ShowVisualElement(_screenItemComponent.pauseScreen, false);
        ShowVisualElement(_screenItemComponent.timerScreen, true);
        //here
        StartCoroutine(TimerToResumeGame());
        GameManager.Instance.GameResume(() =>
        {
            BlurBackground(false);
            ShowVisualElement(_screenItemComponent.timerScreen, false);
        });
    }
    private IEnumerator TimerToResumeGame()
    {
        float duration = GameManager.Instance.TimeToResumeGame + 0.5f;
        while (duration >= 0f)
        {
            duration -= Time.deltaTime;
            _screenItemComponent.timerText.text = $"{(int)duration}";
            yield return null;
        }
    }
    private void ReturnToMenu(ClickEvent evt)
    {
        GameManager.Instance.StopCoroutinesTime();

        ShowVisualElement(_screenItemComponent.pauseScreen, false);
        ShowVisualElement(_screenItemComponent.abandonScreen, true);

        ReplaceGameScoreScreen(_screenItemComponent.abandonText, _highestScoreLocalizationSubString, DataManager.OnGetScore());
        Invoke("AbandonGame", GameManager.Instance.TimeToResumeGame);
    }
    private void AbandonGame()
    {
        InitScreens();
        GameStateController.OnResetGame?.Invoke();

        BlurBackground(false);
    }
    private void GameOver()
    {
        ShowVisualElement(_screenItemComponent.gameScreen, false);
        ShowVisualElement(_screenItemComponent.gameOverScreen, true);

        ReplaceGameScoreScreen(_screenItemComponent.finalHighestScoreText, _highestScoreLocalizationSubString, DataManager.OnGetScore());
        ReplaceGameScoreScreen(_screenItemComponent.finalScoreText, _finalScoreLocalizationSubString, GameManager.Instance.CurrentScore);

        BlurBackground(true);
    }
    private void ShowMenuScreen(ClickEvent evt)
    {
        InitScreens();
        ReplaceMenuGameScoreScreen();

        GameStateController.OnResetGame?.Invoke();

        BlurBackground(false);
    }

    private void QuitGame(ClickEvent evt)
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
        Application.Quit();
    }
    #endregion
    #region Screen Behaviour
    private void BlurBackground(bool state)
    {
        if (_volume == null)
            return;

        DepthOfField blurDOF;
        if (_volume.profile.TryGet<DepthOfField>(out blurDOF))
        {
            blurDOF.active = state;
        }
    }
    private void ShowVisualElement(VisualElement visualElement, bool state)
    {
        if (visualElement == null)
            return;

        visualElement.style.display = (state) ? DisplayStyle.Flex : DisplayStyle.None;
    }
    #endregion
}