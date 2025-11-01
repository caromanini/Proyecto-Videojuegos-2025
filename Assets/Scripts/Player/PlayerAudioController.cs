using UnityEngine;

public class PlayerAudioController : MonoBehaviour
{
    [Header("Audio Clips")]
    [SerializeField] private AudioClip footstepSound;
    [SerializeField] private AudioClip collisionSound;

    private PlayerInputController inputController;
    private PlayerPhysicsController physicsController;
    private PlayerStateController stateController;

    private AudioSource footstepAudioSource;
    private AudioSource collisionAudioSource;

    private bool isPlayingFootsteps = false;

    void Awake()
    {
        inputController = GetComponent<PlayerInputController>();
        physicsController = GetComponent<PlayerPhysicsController>();
        stateController = GetComponent<PlayerStateController>();

        footstepAudioSource = gameObject.AddComponent<AudioSource>();
        footstepAudioSource.playOnAwake = false;
        footstepAudioSource.clip = footstepSound;
        footstepAudioSource.loop = true;

        collisionAudioSource = gameObject.AddComponent<AudioSource>();
        collisionAudioSource.playOnAwake = false;
        collisionAudioSource.clip = collisionSound;
        collisionAudioSource.loop = false;
    }

    void Update()
    {
        UpdateFootsteps();
    }

    private void UpdateFootsteps()
    {
        bool shouldPlayFootsteps = ShouldPlayFootsteps();

        if (shouldPlayFootsteps && !isPlayingFootsteps)
        {
            StartFootsteps();
        }
        else if (!shouldPlayFootsteps && isPlayingFootsteps)
        {
            StopFootsteps();
        }
    }

    private bool ShouldPlayFootsteps()
    {
        // No reproducir si está stunned
        if (stateController != null && !stateController.CanMove())
        {
            return false;
        }

        // No reproducir si está colisionando
        if (physicsController != null && physicsController.IsColliding())
        {
            return false;
        }

        // Reproducir si se está moviendo
        if (inputController != null && inputController.IsMoving())
        {
            return true;
        }

        return false;
    }

    private void StartFootsteps()
    {
        if (footstepAudioSource != null && footstepSound != null)
        {
            footstepAudioSource.Play();
            isPlayingFootsteps = true;
        }
    }

    public void StopFootsteps()
    {
        if (footstepAudioSource != null)
        {
            footstepAudioSource.Stop();
            isPlayingFootsteps = false;
        }
    }

    public void PlayCollisionSound()
    {
        PlayCollisionSound(0f);
    }

    public void PlayCollisionSound(float timeOffset)
    {
        if (collisionAudioSource != null && collisionSound != null)
        {
            collisionAudioSource.time = timeOffset;
            collisionAudioSource.Play();
        }
    }
}