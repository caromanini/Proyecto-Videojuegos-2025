using UnityEngine;

public class EyeDropsItem : PowerUpBase
{
    [Header("Eye Drops Settings")]
    [SerializeField] private float visionDuration = 5f;

    protected override void ApplyPowerUpEffect()
    {
        Collector collector = FindPlayerComponent<Collector>();

        if (collector != null)
        {
            collector.ApplyTemporaryFullGlasses(visionDuration);
        }
    }
}