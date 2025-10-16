using UnityEngine;

public class PillowItem : MonoBehaviour, IItem   
{
    [SerializeField] private AudioClip pickupSfx;

    public void Collect()                        
    {
        var player = FindAnyObjectByType<PlayerMovement>();
        if (player != null)
        {
            player.SetHasPillow(true);           
        }

        if (pickupSfx != null)
        {
            AudioSource.PlayClipAtPoint(pickupSfx, transform.position);
        }
        Destroy(gameObject);
    }
}
