using UnityEngine;
using UnityEngine.SceneManagement;

// Este script es para manejar los estados del juego: Playing, Win, Lose
// y sus pantallas
public class GameStateManager : MonoBehaviour
{
    [Header("UI Screens")]
    [SerializeField] private GameObject gameOverScreen;
    [SerializeField] private GameObject winScreen;

    public enum GameState
    {
        Playing,
        Won,
        Lost,
        Paused
    }

    private GameState currentState = GameState.Playing;

    public GameState CurrentState => currentState;

    void Start()
    {
        InitializeGameState();
        SubscribeToEvents();
    }

    void OnDestroy()
    {
        UnsubscribeFromEvents();
    }

    private void InitializeGameState()
    {
        currentState = GameState.Playing;
        Time.timeScale = 1f;

        if (gameOverScreen != null) gameOverScreen.SetActive(false);
        if (winScreen != null) winScreen.SetActive(false);
    }

    private void SubscribeToEvents()
    {
        if (GameEventManager.Instance != null)
        {
            GameEventManager.Instance.OnPlayerWin += OnPlayerWin;
            GameEventManager.Instance.OnPlayerLose += OnPlayerLose;
        }
    }

    private void UnsubscribeFromEvents()
    {
        if (GameEventManager.Instance != null)
        {
            GameEventManager.Instance.OnPlayerWin -= OnPlayerWin;
            GameEventManager.Instance.OnPlayerLose -= OnPlayerLose;
        }
    }

    private void OnPlayerWin()
    {
        if (currentState != GameState.Playing) return;

        currentState = GameState.Won;
        ShowWinScreen();
    }

    private void OnPlayerLose()
    {
        if (currentState != GameState.Playing) return;

        currentState = GameState.Lost;
        ShowGameOverScreen();
    }

    private void ShowWinScreen()
    {
        if (winScreen != null)
        {
            winScreen.SetActive(true);
        }

        Time.timeScale = 0f;
        Debug.Log("[GameStateManager] WIN!");
    }

    private void ShowGameOverScreen()
    {
        if (gameOverScreen != null)
        {
            gameOverScreen.SetActive(true);
        }

        Debug.Log("[GameStateManager] LOSE!");
    }

    public void RestartLevel()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void LoadMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }

    public void PauseGame()
    {
        if (currentState == GameState.Playing)
        {
            currentState = GameState.Paused;
            Time.timeScale = 0f;
        }
    }

    public void ResumeGame()
    {
        if (currentState == GameState.Paused)
        {
            currentState = GameState.Playing;
            Time.timeScale = 1f;
        }
    }
}