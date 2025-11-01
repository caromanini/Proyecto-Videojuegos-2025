using UnityEngine;

public class Exit : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (GameEventManager.Instance != null)
            {
                GameEventManager.Instance.TriggerPlayerWin();
            }
            else
            {
                Debug.LogWarning("Exit: GameEventManager no encontrado.");
            }
        }
    }
}