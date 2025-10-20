using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections; // Necesario para IEnumerator

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    private Rigidbody2D rb;
    private Vector2 moveInput;
    private Animator animator;

    [Header("Stun")]
    [SerializeField] private float stunDuration = 3f;
    private bool isStunned = false;
    private float stunTimer = 0f;
    

    // Almohada
    [Header("Pillow")]
    [SerializeField] private bool hasPillow = false;
    [SerializeField] private GameObject pillowIcon = null;

    // --- AUDIO ---
    [Header("Audio")]
    [SerializeField] private AudioClip footstepSound; // Sonido de pasos (bucle)
    [SerializeField] private AudioClip collisionSound; // Sonido de colisión (puntual)

    // Dos AudioSources separados para sonidos en bucle y puntuales
    private AudioSource footstepAudioSource;
    private AudioSource collisionAudioSource;
    private bool isPlayingFootsteps = false;


    // --- ESTADO ---
    private bool isColliding = false;
    private float collisionTimer = 0f;
    private bool speedReducedThisCollision = false;

    private bool isSpeedBoostActive = false;
    private bool controlsInverted = false;

    public void SetHasPillow(bool value)
    {
        hasPillow = value;
        if (pillowIcon != null) pillowIcon.SetActive(hasPillow);
    }


    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        // CONFIGURACIÓN DE AUDIO SOURCE PARA PASOS (bucle)
        footstepAudioSource = gameObject.AddComponent<AudioSource>();
        footstepAudioSource.playOnAwake = false;
        footstepAudioSource.clip = footstepSound; // Asignar el clip de pasos
        footstepAudioSource.loop = true; // DEBE REPETIRSE

        // CONFIGURACIÓN DE AUDIO SOURCE PARA COLISIONES (puntual)
        collisionAudioSource = gameObject.AddComponent<AudioSource>();
        collisionAudioSource.playOnAwake = false;
        collisionAudioSource.clip = collisionSound; // Asignar el clip de colisión
        collisionAudioSource.loop = false; // NO DEBE REPETIRSE
    }

    void Update()
    {
        if (isStunned)
        {
            stunTimer -= Time.deltaTime;
            if (stunTimer < 0f)
            {
                isStunned = false;
            }
            moveInput = Vector2.zero;
            rb.linearVelocity = Vector2.zero;
            if(isPlayingFootsteps) StopFootsteps();
            if(animator) animator.SetBool("isWalking", false);
            return;
        }

        rb.linearVelocity = moveInput * moveSpeed;

        // Lógica de Colisión (Penalización y Sonido)
        if (isColliding)
        {
            collisionTimer += Time.deltaTime;

            if (collisionTimer >= 0.1f && !speedReducedThisCollision)
            {
                moveSpeed *= 0.9f;
                speedReducedThisCollision = true;

                // Reproducir el sonido de colisión
                if (collisionAudioSource && collisionSound)
                {
                    // Usa el AudioSource de colisión para el efecto de sonido
                    collisionAudioSource.time = 0.8f; // saltar los primeros 0.8s
                    collisionAudioSource.Play();
                }
            }
        }

        // Lógica del Sonido de Pasos
        bool isMoving = moveInput.magnitude > 0.1f;

        // Solo inicia los pasos si se mueve y no está colisionando
        if (isMoving && !isPlayingFootsteps && !isColliding)
        {
            StartFootsteps();
        }
        else if (!isMoving && isPlayingFootsteps)
        {
            StopFootsteps();
        }
    }

    private void StartFootsteps()
    {
        if (footstepAudioSource && footstepSound)
        {
            // El clip ya está asignado en Start(), solo reproducimos
            footstepAudioSource.Play();
            isPlayingFootsteps = true;
        }
    }

    private void StopFootsteps()
    {
        if (footstepAudioSource)
        {
            footstepAudioSource.Stop();
            isPlayingFootsteps = false;
        }
    }


    // --- Detectar colisiones ---
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("NPC")) // detectar la colision con npc
        {
            if (hasPillow)
            {
                hasPillow = false;
                if (pillowIcon != null) pillowIcon.SetActive(false);
                return;
            }
            Stun(stunDuration);
            StopFootsteps();

            if(collisionAudioSource && collisionSound)
            {
                collisionAudioSource.time = 0.0f;
                collisionAudioSource.Play();
            }
            return;
        }

        if (hasPillow)
        {
            hasPillow = false;
            if (pillowIcon != null) pillowIcon.SetActive(false);
            return;
        }

        if (isSpeedBoostActive)
        {
            controlsInverted = true;
        }
        else
        {
            // Detenemos los pasos al colisionar (aunque collisionAudioSource no los interrumpe,
            // es bueno detener el bucle para dar prioridad al sonido puntual y a la pausa)
            StopFootsteps();

            isColliding = true;
            collisionTimer = 0f;
            speedReducedThisCollision = false;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        isColliding = false; 
        // Si el jugador todavía se está moviendo, reiniciamos los pasos
        if (moveInput.magnitude > 0.1f)
        {
            StartFootsteps();
        }
    }

    public void Move(InputAction.CallbackContext context)
    {
        if (isStunned)
        {
            if(context.canceled && animator)
            {
                animator.SetBool("isWalking", false);
                animator.SetFloat("LastInputX", 0f);
                animator.SetFloat("LastInputY", 0f);

            }
            return;
        }
        animator.SetBool("isWalking", true);

        if (context.canceled)
        {
            animator.SetBool("isWalking", false);
            animator.SetFloat("LastInputX", moveInput.x);
            animator.SetFloat("LastInputY", moveInput.y);
        }

        moveInput = context.ReadValue<Vector2>();
        if (controlsInverted)
        {
            moveInput = -moveInput;
        }

        animator.SetFloat("InputX", moveInput.x);
        animator.SetFloat("InputY", moveInput.y);
    }

    public void ActivateSpeedBoost(float newSpeed, float duration)
    {
        StartCoroutine(SpeedBoostCoroutine(newSpeed, duration));
    }

    private System.Collections.IEnumerator SpeedBoostCoroutine(float newSpeed, float duration)
    {
        float originalSpeed = moveSpeed;
        moveSpeed = newSpeed;
        isSpeedBoostActive = true;

        yield return new WaitForSeconds(duration);

        isSpeedBoostActive = false;
        controlsInverted = false;
        moveSpeed = originalSpeed;
    }

    private void Stun(float duration)
    {
        isStunned = true;
        stunTimer = duration;
        rb.linearVelocity = Vector2.zero;

        StopFootsteps();
        if (animator) animator.SetBool("isWalking", false);
    }

}