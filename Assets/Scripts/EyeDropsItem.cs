using UnityEngine;

public class EyeDropsItem : MonoBehaviour, IItem
{
    [SerializeField] private float duration = 5f;
    [SerializeField] private AudioClip pickupSfx;

    public void Collect()
    {
        var collector = FindAnyObjectByType<Collector>();
        if (collector != null)
        {
            collector.ApplyTemporaryFullGlasses(duration);
        }
        else
        {
            Debug.LogWarning("EyeDropsItem: no se encontró Collector en la escena.");
        }

        if (pickupSfx != null)
        {
            AudioSource.PlayClipAtPoint(pickupSfx, transform.position);
        }

        Destroy(gameObject);
    }
}