using System;
using UnityEngine;

// Este script maneja los eventos del juego con el patron Observer.
// Es para que los distintos sistemas reaccionen a eventos sin estar acoplados directamente.
public class GameEventManager : MonoBehaviour
{
    public static GameEventManager Instance { get; private set; }

    public event Action OnPlayerWin;
    public event Action OnPlayerLose;
    public event Action<int> OnGlassPieceCollected;
    public event Action<string> OnItemCollected;
    public event Action<string> OnPlayerCollision;
    public event Action<int, int> OnProgressChanged;
    public event Action OnExitActivated;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void TriggerPlayerWin()
    {
        Debug.Log("[GameEventManager] WIN!");
        OnPlayerWin?.Invoke();
    }

    public void TriggerPlayerLose()
    {
        Debug.Log("[GameEventManager] LOSE!");
        OnPlayerLose?.Invoke();
    }

    public void TriggerGlassPieceCollected(int amount)
    {
        Debug.Log($"[GameEventManager] Glass Piece Collected: {amount}");
        OnGlassPieceCollected?.Invoke(amount);
    }

    public void TriggerItemCollected(string itemName)
    {
        Debug.Log($"[GameEventManager] Item Collected: {itemName}");
        OnItemCollected?.Invoke(itemName);
    }

    public void TriggerPlayerCollision(string collisionType)
    {
        OnPlayerCollision?.Invoke(collisionType);
    }

    public void TriggerProgressChanged(int currentProgress, int totalProgress)
    {
        OnProgressChanged?.Invoke(currentProgress, totalProgress);
    }

    public void TriggerExitActivated()
    {
        Debug.Log("[GameEventManager] Exit Activated!");
        OnExitActivated?.Invoke();
    }

    void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }
}