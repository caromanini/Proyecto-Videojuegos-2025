using UnityEngine;
using TMPro;

public class TimerManager : MonoBehaviour
{
    [Header("Timer Settings")]
    [SerializeField] private float totalTime = 60f;
    [SerializeField] private TMP_Text timerText;

    private float timeRemaining;
    private bool timerRunning = true;

    public float TotalTime => totalTime;
    public float TimeRemaining => timeRemaining;
    public bool IsTimeUp => timeRemaining <= 0f;

    void Start()
    {
        InitializeTimer();
    }

    void Update()
    {
        if (timerRunning)
        {
            UpdateTimer();
        }
    }

    private void InitializeTimer()
    {
        timeRemaining = totalTime;
        UpdateTimerDisplay();
    }

    private void UpdateTimer()
    {
        if (timeRemaining > 0)
        {
            timeRemaining -= Time.deltaTime;
            UpdateTimerDisplay();
        }
        else
        {
            OnTimeUp();
        }
    }

    private void UpdateTimerDisplay()
    {
        if (timerText == null) return;

        timeRemaining = Mathf.Max(0, timeRemaining);

        int minutes = Mathf.FloorToInt(timeRemaining / 60);
        int seconds = Mathf.FloorToInt(timeRemaining % 60);

        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    private void OnTimeUp()
    {
        timeRemaining = 0;
        timerRunning = false;

        Debug.Log("[TimerManager] Time's Up!");

        // Notificar al GameEventManager que el jugador perdió
        if (GameEventManager.Instance != null)
        {
            GameEventManager.Instance.TriggerPlayerLose();
        }
    }

    public void StopTimer()
    {
        timerRunning = false;
    }

    public void ResetTimer()
    {
        timeRemaining = totalTime;
        timerRunning = true;
        UpdateTimerDisplay();
    }

    public float GetRemainingFraction()
    {
        return Mathf.Clamp01(timeRemaining / totalTime);
    }

    public void AddTime(float seconds)
    {
        timeRemaining += seconds;
        timeRemaining = Mathf.Min(timeRemaining, totalTime);
    }
}