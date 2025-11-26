using UnityEngine;

public class SpeedBoostItem : PowerUpBase
{
    [Header("Speed Boost Settings")]
    [SerializeField] private float boostedSpeed = 15f;
    [SerializeField] private float boostDuration = 10f;

    protected override void ApplyPowerUpEffect()
    {
        PlayerPhysicsController physicsController = FindPlayerComponent<PlayerPhysicsController>();

        if (physicsController != null)
        {
            physicsController.ActivateSpeedBoost(boostedSpeed, boostDuration);
        }

        PlayerStateController stateController = FindPlayerComponent<PlayerStateController>();
        if (stateController != null)
        {
            stateController.ActivateSpeedBoost(boostDuration);
        }
    }
}