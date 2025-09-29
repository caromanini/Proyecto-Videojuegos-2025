using UnityEngine;

public class SpeedPowerUp : MonoBehaviour
{
    [SerializeField] private float boostedSpeed = 15f;
    [SerializeField] private float duration = 10f;

    private void OnTriggerEnter2D(Collider2D other)
    {
        PlayerMovement player = other.GetComponent<PlayerMovement>();
        if (player != null)
        {
            player.ActivateSpeedBoost(boostedSpeed, duration);
            Destroy(gameObject); 
        }
    }
}
