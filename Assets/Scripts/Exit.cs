using UnityEngine;

public class Exit : MonoBehaviour
{
    [SerializeField] private GameController gameController; // arrástralo en el Inspector o lo buscará solo

    private void Start()
    {
        // Si no le asignamos el GameController desde el Inspector,
        // lo busca automáticamente en la escena.
        if (gameController == null)
        {
            gameController = FindObjectOfType<GameController>();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Solo reacciona si el que entra al trigger tiene el tag "Player"
        if (other.CompareTag("Player"))
        {
            if (gameController != null)
            {
                gameController.PlayerReachedExit();
            }
            else
            {
                Debug.LogWarning("Exit: no se encontró GameController.");
            }
        }
    }
}
