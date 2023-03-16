using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class ScreenItemComponent
{
    private const string PLAY_BUTTON = "play_Key";
    private const string PAUSE_BUTTON = "pause_Key";
    private const string RESUME_BUTTON = "resume_Key";
    private const string PAUSE_QUIT_BUTTON = "pause_quit_Key";
    private const string GAME_OVER_QUIT_BUTTON = "end-game-Key";
    private const string QUIT_GAME_BUTTON = "quit_game_Key";

    private const string MENU_HIGHEST_SCORE_TEXT = "menu_highest_score_Key";
    private const string GAME_HIGHEST_SCORE_TEXT = "game_highest_score_Key";
    private const string FINAL_HIGHEST_SCORE_TEXT = "final_highest_score_Key";
    private const string CURRENT_SCORE_TEXT = "current_score_Key";
    private const string FINAL_SCORE_TEXT = "final_score_Key";
    private const string TIMER_TEXT = "pause_timer_Key";
    private const string ABANDON_TEXT = "abandon_score_Key";

    private const string MENU_SCREEN = "menu-panel";
    private const string GAME_SCREEN = "game-panel";
    private const string PAUSE_SCREEN = "pause-panel";
    private const string UNPAUSE_SCREEN = "unpause-panel";
    private const string GAME_OVER_SCREEN = "game_over-panel";
    private const string ABANDON_PANEL = "abandon-panel";
    private const string QUITE_GAME_PANEL = "quit_game-panel";

    //buttons
    public Button playButton;
    public Button pauseButton;
    public Button resumeButton;
    public Button pauseQuitButton;
    public Button gameOverQuitButton;
    public Button quitGameButton;

    //labels (texts)
    public Label menuHighestScoreText;
    public Label gameHighestScoreText;
    public Label finalHighestScoreText;
    public Label currentScoreText;
    public Label finalScoreText;
    public Label timerText;
    public Label abandonText;

    //visual elements (panels)
    public VisualElement menuScreen;
    public VisualElement gameScreen;
    public VisualElement pauseScreen;
    public VisualElement timerScreen;
    public VisualElement gameOverScreen;
    public VisualElement abandonScreen;
    public VisualElement quitGameScreen;

    public void SetVisualElements(VisualElement visualElement)
    {
        playButton = visualElement.Q<Button>(PLAY_BUTTON);
        pauseButton = visualElement.Q<Button>(PAUSE_BUTTON);
        resumeButton = visualElement.Q<Button>(RESUME_BUTTON);
        pauseQuitButton = visualElement.Q<Button>(PAUSE_QUIT_BUTTON);
        gameOverQuitButton = visualElement.Q<Button>(GAME_OVER_QUIT_BUTTON);
        quitGameButton = visualElement.Q<Button>(QUIT_GAME_BUTTON);

        menuHighestScoreText = visualElement.Q<Label>(MENU_HIGHEST_SCORE_TEXT);
        gameHighestScoreText = visualElement.Q<Label>(GAME_HIGHEST_SCORE_TEXT);
        finalHighestScoreText = visualElement.Q<Label>(FINAL_HIGHEST_SCORE_TEXT);
        currentScoreText = visualElement.Q<Label>(CURRENT_SCORE_TEXT);
        finalScoreText = visualElement.Q<Label>(FINAL_SCORE_TEXT);
        timerText = visualElement.Q<Label>(TIMER_TEXT);
        abandonText = visualElement.Q<Label>(ABANDON_TEXT);

        menuScreen = visualElement.Q(MENU_SCREEN);
        gameScreen = visualElement.Q(GAME_SCREEN);
        pauseScreen = visualElement.Q(PAUSE_SCREEN);
        timerScreen = visualElement.Q(UNPAUSE_SCREEN);
        gameOverScreen = visualElement.Q(GAME_OVER_SCREEN);
        abandonScreen = visualElement.Q(ABANDON_PANEL);
        quitGameScreen = visualElement.Q(QUITE_GAME_PANEL);
    }
}