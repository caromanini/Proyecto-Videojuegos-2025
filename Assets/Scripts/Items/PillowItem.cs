using UnityEngine;

public class PillowItem : PowerUpBase
{
    protected override void ApplyPowerUpEffect()
    {
        PlayerStateController stateController = FindPlayerComponent<PlayerStateController>();

        if (stateController != null)
        {
            stateController.SetHasPillow(true);
        }
    }
}