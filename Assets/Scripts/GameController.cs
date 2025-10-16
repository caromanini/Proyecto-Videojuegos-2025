using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    public int progressAmount;
    public Slider progressSlider;
    public AudioSource bgmAudioSource; // NUEVO: Referencia al AudioSource

    [Header("Timer")]
    public float totalTime = 60f;
    private float timeRemaining;
    public TMP_Text timerText;

    [Header("UI Screens")]
    public GameObject gameOverScreen;
    public GameObject winScreen;

    [Header("Exit Settings")]
    [SerializeField] private GameObject exitPrefab;         
    [SerializeField] private Transform exitSpawnPoint;      
    private GameObject exitInstance;

    public float TotalTime => totalTime;
    public float TimeRemaining => timeRemaining;

    void Start()
    {
        progressAmount = 0;
        progressSlider.value = 0;
        Gem.OnGemCollect += IncreaseProgressAmount;

        timeRemaining = totalTime;
        gameOverScreen.SetActive(false);
        winScreen.SetActive(false);
        StartBGM();
    }

    void Update()
    {
        UpdateTimer();
    }

    void StartBGM()
    {
        if (bgmAudioSource != null)
        {

            bgmAudioSource.volume = 0.3f;
            bgmAudioSource.loop = true; // Asegura que se repita
            bgmAudioSource.Play();
        }
    }

    void StopBGM()
    {
        if (bgmAudioSource != null && bgmAudioSource.isPlaying)
        {
            bgmAudioSource.Stop();
        }
    }

    public void ResetGame()
    {
        gameOverScreen.SetActive(false);
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    void IncreaseProgressAmount(int amount)
    {
        progressAmount += amount;
        progressSlider.value = progressAmount;

        if (progressAmount >= 4)
        {
            // Level complete
            // Debug.Log("Level Complete");
            // YouWon();
            ActivateExit();
        }
    }

    void ActivateExit()
    {
        if (exitInstance != null) return;

        if (exitSpawnPoint != null && exitPrefab != null)
        {
            exitInstance = Instantiate(exitPrefab, exitSpawnPoint.position, Quaternion.identity);
            Debug.Log("Salida creada en " + exitSpawnPoint.position);
        }
        else
        {
            Debug.LogWarning("Falta asignar exitPrefab o exitSpawnPoint en GameController.");
        }
    }

    public void PlayerReachedExit()
    {
        YouWon();   
    }


    void UpdateTimer()
    {
        if (timeRemaining > 0)
        {
            timeRemaining -= Time.deltaTime;

            int minutes = Mathf.FloorToInt(timeRemaining / 60);
            int seconds = Mathf.FloorToInt(timeRemaining % 60);

            timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
        }
        else
        {
            timeRemaining = 0;
            timerText.text = "00:00";
            GameOverScreen();
        }
    }

    void GameOverScreen()
    {
        StopBGM();
        gameOverScreen.SetActive(true);
    }

    void YouWon()
    {
        StopBGM();
        winScreen.SetActive(true);
        Time.timeScale = 0f;
    }

    void OnDestroy()
    {
        Gem.OnGemCollect -= IncreaseProgressAmount;
    }

    public float GetRemainingFraction()
    {
        return Mathf.Clamp01(timeRemaining / totalTime);
    }


}
