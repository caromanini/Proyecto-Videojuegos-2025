using UnityEngine;

public class ExitManager : MonoBehaviour
{
    [Header("Exit Settings")]
    [SerializeField] private GameObject exitPrefab;
    [SerializeField] private Transform exitSpawnPoint;

    private GameObject exitInstance;
    private bool exitActivated = false;

    public bool IsExitActivated => exitActivated;

    void Start()
    {
        SubscribeToEvents();
    }

    void OnDestroy()
    {
        UnsubscribeFromEvents();
    }

    private void SubscribeToEvents()
    {
        if (GameEventManager.Instance != null)
        {
            GameEventManager.Instance.OnExitActivated += ActivateExit;
        }
    }

    private void UnsubscribeFromEvents()
    {
        if (GameEventManager.Instance != null)
        {
            GameEventManager.Instance.OnExitActivated -= ActivateExit;
        }
    }

    public void ActivateExit()
    {
        if (exitActivated)
        {
            return;
        }

        if (!ValidateExitSetup())
        {
            return;
        }

        SpawnExit();
        exitActivated = true;

        Debug.Log($"[ExitManager] Exit activated at {exitSpawnPoint.position}");
    }

    private bool ValidateExitSetup()
    {
        if (exitPrefab == null)
        {
            return false;
        }

        if (exitSpawnPoint == null)
        {
            return false;
        }

        return true;
    }

    private void SpawnExit()
    {
        exitInstance = Instantiate(
            exitPrefab,
            exitSpawnPoint.position,
            Quaternion.identity
        );
    }

    public void DeactivateExit()
    {
        if (exitInstance != null)
        {
            Destroy(exitInstance);
            exitInstance = null;
        }

        exitActivated = false;
    }
}