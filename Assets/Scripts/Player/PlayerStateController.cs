using UnityEngine;
using System.Collections;

// Este script controla los estaods del jugador: stun, escudo/almohada, speed boost, controles invertidos
public class PlayerStateController : MonoBehaviour
{
    [Header("Stun Settings")]
    [SerializeField] private float stunDuration = 3f;

    [Header("Pillow Settings")]
    [SerializeField] private GameObject pillowIcon;

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
        yield return new WaitForSeconds(duration);
        IsSpeedBoostActive = false;
        ControlsInverted = false;
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