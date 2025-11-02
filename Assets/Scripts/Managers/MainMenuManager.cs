using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    [Header("Audio")]
    [SerializeField] private AudioSource menuMusicSource;

    [Header("Panels")]
    [SerializeField] private GameObject mainPanel;

    private AudioSource sfxSource;

    void Start()
    {
        InitializeMenu();
    }

    private void InitializeMenu()
    {
        Time.timeScale = 1f;

        ShowMainPanel();

        // Tengo que poner musica para el mainmenu en Unity
        if (menuMusicSource != null && !menuMusicSource.isPlaying)
        {
            menuMusicSource.Play();
        }

        sfxSource = gameObject.AddComponent<AudioSource>();
        sfxSource.playOnAwake = false;
    }

    // Por ahora se va directamente al level 1
    public void PlayGame()
    {
        SceneManager.LoadScene("Level_1");
    }

    public void LoadLevel(string levelName)
    {
        SceneManager.LoadScene(levelName);
    }

    public void LoadLevelByIndex(int levelIndex)
    {
        SceneManager.LoadScene(levelIndex);
    }

    public void ShowMainPanel()
    {
        if (mainPanel != null) mainPanel.SetActive(true);
    }

    public void QuitGame()
    {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    public void SetMusicVolume(float volume)
    {
        if (menuMusicSource != null)
        {
            menuMusicSource.volume = Mathf.Clamp01(volume);
        }
    }

    public void SetSFXVolume(float volume)
    {
        if (sfxSource != null)
        {
            sfxSource.volume = Mathf.Clamp01(volume);
        }
    }
}