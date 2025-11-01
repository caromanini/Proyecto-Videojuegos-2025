using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputController : MonoBehaviour
{
    private PlayerStateController stateController;
    private PlayerPhysicsController physicsController;
    private Animator animator;

    public Vector2 MoveInput { get; private set; }

    void Awake()
    {
        stateController = GetComponent<PlayerStateController>();
        physicsController = GetComponent<PlayerPhysicsController>();
        animator = GetComponent<Animator>();
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        // Si el jugador esta stunned, no se procesa el input
        if (stateController != null && !stateController.CanMove())
        {
            HandleStunnedInput(context);
            return;
        }

        Vector2 rawInput = context.ReadValue<Vector2>();

        if (stateController != null && stateController.ControlsInverted)
        {
            rawInput = -rawInput;
        }

        MoveInput = rawInput;

        if (physicsController != null)
        {
            physicsController.SetMoveDirection(MoveInput);
        }

        UpdateAnimations(context);
    }

    private void HandleStunnedInput(InputAction.CallbackContext context)
    {
        if (context.canceled && animator != null)
        {
            animator.SetBool("isWalking", false);
            animator.SetFloat("LastInputX", 0f);
            animator.SetFloat("LastInputY", 0f);
        }
        MoveInput = Vector2.zero;
    }

    private void UpdateAnimations(InputAction.CallbackContext context)
    {
        if (animator == null) return;

        if (context.canceled)
        {
            animator.SetBool("isWalking", false);
            animator.SetFloat("LastInputX", MoveInput.x);
            animator.SetFloat("LastInputY", MoveInput.y);
        }
        else
        {
            animator.SetBool("isWalking", true);
            animator.SetFloat("InputX", MoveInput.x);
            animator.SetFloat("InputY", MoveInput.y);
        }
    }

    public bool IsMoving()
    {
        return MoveInput.magnitude > 0.1f;
    }
}