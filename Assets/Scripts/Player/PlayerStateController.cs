using UnityEngine;
using System.Collections;

// Este script controla los estaods del jugador: stun, escudo/almohada, speed boost, controles invertidos
public class PlayerStateController : MonoBehaviour
{
    [Header("Stun Settings")]
    [SerializeField] private float stunDuration = 3f;

    [Header("UI Icons")]
    [SerializeField] private GameObject pillowIcon;
    [SerializeField] private GameObject energyDrinkIcon;
    [SerializeField] private GameObject eyeDropsIcon;

    public bool IsStunned { get; private set; }
    public bool HasPillow { get; private set; }
    public bool IsSpeedBoostActive { get; private set; }
    public bool ControlsInverted { get; private set; }

    private float stunTimer = 0f;

    void Update()
    {
        UpdateStunTimer();
    }

    private void UpdateStunTimer()
    {
        if (IsStunned)
        {
            stunTimer -= Time.deltaTime;
            if (stunTimer <= 0f)
            {
                IsStunned = false;
            }
        }
    }

    public void ApplyStun(float duration)
    {
        IsStunned = true;
        stunTimer = duration;
    }

    public void ApplyStun()
    {
        ApplyStun(stunDuration);
    }

    public void SetHasPillow(bool value)
    {
        HasPillow = value;
        if (pillowIcon != null)
        {
            pillowIcon.SetActive(value);
        }
    }

    public void ConsumePillow()
    {
        if (HasPillow)
        {
            SetHasPillow(false);
        }
    }

    public void ActivateSpeedBoost(float duration)
    {
        StartCoroutine(SpeedBoostCoroutine(duration));
    }

    private IEnumerator SpeedBoostCoroutine(float duration)
    {
        IsSpeedBoostActive = true;

        if (energyDrinkIcon != null) energyDrinkIcon.SetActive(true);

        yield return new WaitForSeconds(duration);

        if (energyDrinkIcon != null) energyDrinkIcon.SetActive(false);
        
        IsSpeedBoostActive = false;
        ControlsInverted = false;
    }

    public void SetEyeDropsActive(bool isActive)
    {
        if (eyeDropsIcon != null)
        {
            eyeDropsIcon.SetActive(isActive);
        }
    }

    public void InvertControls()
    {
        ControlsInverted = true;
    }

    public bool CanMove()
    {
        return !IsStunned;
    }
}