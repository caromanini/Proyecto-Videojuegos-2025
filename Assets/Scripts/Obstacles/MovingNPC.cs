using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody2D))]
public class MovingNPC : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private float moveDistance = 3f;
    [SerializeField] private bool moveHorizontally = true;

    [Header("Speech Bubble")]
    [SerializeField] private GameObject speechBubble;
    [SerializeField] private float bubbleDisplayTime = 3f;

    private Vector2 initialPosition;
    private Vector2 moveDirection;
    private Rigidbody2D rb;
    private bool movingForward = true;
    private bool hasCollidedThisFrame = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.freezeRotation = true;

        initialPosition = transform.position;
        moveDirection = moveHorizontally ? Vector2.right : Vector2.up;

        if (speechBubble != null)
            speechBubble.SetActive(false);
    }

    void Update()
    {
        MoveNPC();
    }

    private void MoveNPC()
    {
        float distance = Vector2.Distance(transform.position, initialPosition);

        if (distance >= moveDistance)
        {
            movingForward = !movingForward;
            moveDirection = -moveDirection;

            Vector3 scale = transform.localScale;
            scale.x = Mathf.Sign(moveDirection.x) != 0 ? Mathf.Sign(moveDirection.x) * Mathf.Abs(scale.x) : scale.x;
            transform.localScale = scale;
        }

        rb.linearVelocity = moveDirection * moveSpeed;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && !hasCollidedThisFrame)
        {
            hasCollidedThisFrame = true;
            ShowBubble();
            Invoke(nameof(ResetCollision), 0.5f);
        }
    }

    private void ShowBubble()
    {
        if (speechBubble != null)
        {
            speechBubble.SetActive(true);
            StartCoroutine(HideBubbleAfterDelay());
        }
    }

    private IEnumerator HideBubbleAfterDelay()
    {
        yield return new WaitForSeconds(bubbleDisplayTime);

        if (speechBubble != null)
            speechBubble.SetActive(false);
    }

    private void ResetCollision()
    {
        hasCollidedThisFrame = false;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        if (moveHorizontally)
        {
            Gizmos.DrawLine(transform.position - Vector3.right * moveDistance, transform.position + Vector3.right * moveDistance);
        }
        else
        {
            Gizmos.DrawLine(transform.position - Vector3.up * moveDistance, transform.position + Vector3.up * moveDistance);
        }
    }
}
