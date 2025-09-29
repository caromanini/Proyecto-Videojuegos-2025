using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    private Rigidbody2D rb;
    private Vector2 moveInput;
    private Animator animator;

    // --- NUEVO ---
    private bool isColliding = false;
    private float collisionTimer = 0f;
    private bool speedReducedThisCollision = false;

    private bool isSpeedBoostActive = false;
    private bool controlsInverted = false;

    [SerializeField] private AudioClip collisionSound; // Asignar "oof.mp3" en el Inspector
    private AudioSource audioSource;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        // Configurar el audio source
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
        audioSource.clip = collisionSound;
    }

    void Update()
    {
        rb.linearVelocity = moveInput * moveSpeed;

        // Si está colisionando, acumulamos tiempo
        if (isColliding)
        {
            collisionTimer += Time.deltaTime;

            // Ahora basta con 0.1 segundos para aplicar la penalización
            if (collisionTimer >= 0.1f && !speedReducedThisCollision)
            {
                moveSpeed *= 0.9f; // reducir velocidad en 10% permanentemente
                speedReducedThisCollision = true;

                if (audioSource && collisionSound)
                {
                    audioSource.time = 0.8f; // saltar los primeros 0.8s del audio
                    audioSource.Play();
                }
            }
        }
    }

    // --- Detectar colisiones ---
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (isSpeedBoostActive)
        {
            controlsInverted = true;
        }
        else
        {
            isColliding = true;
            collisionTimer = 0f;
            speedReducedThisCollision = false; // permitir reducción en cada nueva colisión
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        isColliding = false;
    }

    public void Move(InputAction.CallbackContext context)
    {
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

}
