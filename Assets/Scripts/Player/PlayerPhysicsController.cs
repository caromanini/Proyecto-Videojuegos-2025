using UnityEngine;
using System.Collections;

public class PlayerPhysicsController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 10f;

    private Rigidbody2D rb;
    private PlayerStateController stateController;
    private PlayerAudioController audioController;
    private Animator animator;
    private Vector2 moveDirection;
    private bool isColliding = false;
    private float collisionTimer = 0f;
    private bool speedReducedThisCollision = false;
    private float originalSpeed;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        stateController = GetComponent<PlayerStateController>();
        audioController = GetComponent<PlayerAudioController>();
        animator = GetComponent<Animator>();

        originalSpeed = moveSpeed;
    }

    void Update()
    {
        UpdateMovement();
        UpdateCollisionPenalty();
    }

    private void UpdateMovement()
    {
        // Si está stunned, no se mueve
        if (stateController != null && !stateController.CanMove())
        {
            rb.linearVelocity = Vector2.zero;
            if (animator != null)
            {
                animator.SetBool("isWalking", false);
            }
            return;
        }

        rb.linearVelocity = moveDirection * moveSpeed;
    }

    private void UpdateCollisionPenalty()
    {
        if (isColliding)
        {
            collisionTimer += Time.deltaTime;

            if (collisionTimer >= 0.1f && !speedReducedThisCollision)
            {
                moveSpeed *= 0.9f;
                speedReducedThisCollision = true;
            }
        }
    }

    public void SetMoveDirection(Vector2 direction)
    {
        moveDirection = direction;
    }

    public void ActivateSpeedBoost(float boostedSpeed, float duration)
    {
        StartCoroutine(SpeedBoostCoroutine(boostedSpeed, duration));
    }

    private IEnumerator SpeedBoostCoroutine(float boostedSpeed, float duration)
    {
        moveSpeed = boostedSpeed;

        if (stateController != null)
        {
            stateController.ActivateSpeedBoost(duration);
        }

        yield return new WaitForSeconds(duration);

        moveSpeed = originalSpeed;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("NPC"))
        {
            HandleNPCCollision();
            return;
        }

        HandleWallCollision();
    }

    private void HandleNPCCollision()
    {
        // Verificar si tiene almohada
        if (stateController != null && stateController.HasPillow)
        {
            stateController.ConsumePillow();
            return;
        }

        if (stateController != null)
        {
            stateController.ApplyStun();
        }

        if (audioController != null)
        {
            audioController.StopFootsteps();
        }

        if (audioController != null)
        {
            audioController.PlayCollisionSound();
        }
    }

    private void HandleWallCollision()
    {
        // Verificar si tiene almohada
        if (stateController != null && stateController.HasPillow)
        {
            stateController.ConsumePillow();
            return;
        }

        // Invertir controles si esta con bebida energetica
        if (stateController != null && stateController.IsSpeedBoostActive)
        {
            stateController.InvertControls();
        }
        else
        {
            isColliding = true;
            collisionTimer = 0f;
            speedReducedThisCollision = false;

            if (audioController != null)
            {
                audioController.StopFootsteps();
                audioController.PlayCollisionSound(0.8f); 
            }
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        isColliding = false;
    }

    public bool IsMoving()
    {
        return moveDirection.magnitude > 0.1f;
    }

    public bool IsColliding()
    {
        return isColliding;
    }
}