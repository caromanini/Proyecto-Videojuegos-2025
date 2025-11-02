using UnityEngine;
using UnityEngine.UI;

// Este script es para manejar el progreso de la recoleccion de lentes
public class ProgressManager : MonoBehaviour
{
    [Header("Progress Settings")]
    [SerializeField] private int targetProgress = 5;
    [SerializeField] private Slider progressSlider;

    private int currentProgress = 0;

    public int CurrentProgress => currentProgress;
    public int TargetProgress => targetProgress;
    public bool IsProgressComplete => currentProgress >= targetProgress;

    void Start()
    {
        InitializeProgress();
        SubscribeToEvents();
    }

    void OnDestroy()
    {
        UnsubscribeFromEvents();
    }

    private void InitializeProgress()
    {
        currentProgress = 0;
        UpdateProgressUI();
    }

    private void SubscribeToEvents()
    {
        if (GameEventManager.Instance != null)
        {
            GameEventManager.Instance.OnGlassPieceCollected += OnGlassPieceCollected;
        }
    }

    private void UnsubscribeFromEvents()
    {
        if (GameEventManager.Instance != null)
        {
            GameEventManager.Instance.OnGlassPieceCollected -= OnGlassPieceCollected;
        }
    }

    private void OnGlassPieceCollected(int amount)
    {
        IncreaseProgress(amount);
    }

    public void IncreaseProgress(int amount)
    {
        currentProgress += amount;
        currentProgress = Mathf.Clamp(currentProgress, 0, targetProgress);

        UpdateProgressUI();

        if (GameEventManager.Instance != null)
        {
            GameEventManager.Instance.TriggerProgressChanged(currentProgress, targetProgress);
        }

        if (IsProgressComplete)
        {
            OnProgressComplete();
        }
    }

    private void UpdateProgressUI()
    {
        if (progressSlider != null)
        {
            progressSlider.maxValue = targetProgress;
            progressSlider.value = currentProgress;
        }
    }

    private void OnProgressComplete()
    {
        Debug.Log("[ProgressManager] COMPLETE");

        // Se notifica con evento para que ExitManager active la salida
        if (GameEventManager.Instance != null)
        {
            GameEventManager.Instance.TriggerExitActivated();
        }
    }

    public void ResetProgress()
    {
        currentProgress = 0;
        UpdateProgressUI();
    }
}