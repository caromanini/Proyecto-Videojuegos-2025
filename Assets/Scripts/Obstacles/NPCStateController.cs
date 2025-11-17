using System.Collections;
using UnityEngine;

public class NPCStateController : MonoBehaviour
{
    public enum NPCState { Patrol, Chasing, Fleeing, Waiting }

    [Header("Settings")]
    [SerializeField] private float waitTimeAtPiece = 4f;
    [SerializeField] private float fleeDuration = 3f;

    private NPCMovementController movementController;
    private NPCVisualController visualController;

    public NPCState CurrentState { get; private set; } = NPCState.Patrol;
    private float stateTimer = 0f;
    private Transform currentTargetTransform;

    void Awake()
    {
        movementController = GetComponent<NPCMovementController>();
        visualController = GetComponent<NPCVisualController>();
    }

    void Start()
    {
        StartCoroutine(LogicLoop());
    }

    void Update()
    {
        if (CurrentState == NPCState.Fleeing)
        {
            stateTimer -= Time.deltaTime;
            if (stateTimer <= 0)
            {
                CurrentState = NPCState.Patrol; 
                movementController.ResumeNormalMovement();
            }
        }
    }

    private IEnumerator LogicLoop()
    {
        yield return new WaitForSeconds(0.5f);

        while (true)
        {
            if (CurrentState == NPCState.Fleeing)
            {
                yield return new WaitForSeconds(0.5f);
                continue;
            }

            GlassPiece[] pieces = FindObjectsOfType<GlassPiece>();
            GameObject player = GameObject.FindGameObjectWithTag("Player");

            if (pieces.Length <= 1)
            {
                HandleAggressiveMode(player);
            }
            else
            {
                yield return StartCoroutine(HandleCollectionMode(pieces));
            }

            yield return new WaitForSeconds(0.5f);
        }
    }

    // Esto es para cuando el NPC persigue al jugador. Por eso "Agresivo"
    private void HandleAggressiveMode(GameObject player)
    {
        CurrentState = NPCState.Chasing;
        if (player != null)
        {
            movementController.SetTargetPosition(player.transform.position);
        }
    }

    private IEnumerator HandleCollectionMode(GlassPiece[] pieces)
    {
        CurrentState = NPCState.Patrol;

        GlassPiece targetPiece = pieces[Random.Range(0, pieces.Length)];
        Vector2 targetPos = targetPiece.transform.position;

        movementController.SetTargetPosition(targetPos);

        while (Vector2.Distance(transform.position, targetPos) > 0.5f)
        {
            if (CurrentState == NPCState.Fleeing) yield break;

            if (targetPiece == null) break;

            yield return null;
        }

        if (CurrentState != NPCState.Fleeing && targetPiece != null)
        {
            CurrentState = NPCState.Waiting;
            yield return new WaitForSeconds(waitTimeAtPiece);
        }
    }

    public void OnCollisionWithPlayer(Transform playerTransform)
    {
        if (CurrentState == NPCState.Fleeing) return;

        CurrentState = NPCState.Fleeing;
        stateTimer = fleeDuration;

        if (visualController != null) visualController.ShowBubble();

        Vector2 dirAway = (transform.position - playerTransform.position).normalized;

        float maxFleeDistance = 5f;

        RaycastHit2D hit = Physics2D.Raycast(transform.position, dirAway, maxFleeDistance);

        Vector2 fleeTarget;

        if (hit.collider != null && !hit.collider.CompareTag("Player") && !hit.collider.CompareTag("NPC"))
        {
            fleeTarget = hit.point - (dirAway * 0.5f);
        }
        else
        {
            fleeTarget = (Vector2)transform.position + (dirAway * maxFleeDistance);
        }

        movementController.FleeTo(fleeTarget);
    }
}