using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class NPCMovementController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private float pushForce = 3f;
    [SerializeField] private float reachThreshold = 0.2f;
    [SerializeField] private float pathUpdateRate = 0.5f;

    private Rigidbody2D rb;
    private NPCStateController stateController;

    private List<Vector2> currentPath = new List<Vector2>();
    private int pathIndex = 0;
    private float pathTimer = 0f;
    private Vector2 currentTargetPos;

    private bool isFleeingMovement = false;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        stateController = GetComponent<NPCStateController>();
        rb.gravityScale = 0;
        rb.freezeRotation = true;
    }

    void Update()
    {
        MoveAlongPath();

        pathTimer -= Time.deltaTime;

        if (pathTimer <= 0 && currentTargetPos != Vector2.zero && !isFleeingMovement)
        {
            CalculatePath(currentTargetPos);
            pathTimer = pathUpdateRate;
        }
    }

    public void SetTargetPosition(Vector2 pos)
    {
        if (isFleeingMovement) return;

        if (Vector2.Distance(pos, currentTargetPos) > 0.5f)
        {
            currentTargetPos = pos;
            CalculatePath(pos);
        }
    }

    public void FleeTo(Vector2 fleePos)
    {
        isFleeingMovement = true;
        currentTargetPos = fleePos;
        SetCollisionWithPlayer(false);
        CalculatePath(fleePos);
    }

    public void ResumeNormalMovement()
    {
        isFleeingMovement = false;
        SetCollisionWithPlayer(true);
        rb.linearVelocity = Vector2.zero;
    }

    public void SetCollisionWithPlayer(bool active)
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            Collider2D myCollider = GetComponent<Collider2D>();
            Collider2D playerCollider = player.GetComponent<Collider2D>();

            if (myCollider != null && playerCollider != null)
            {
                Physics2D.IgnoreCollision(myCollider, playerCollider, !active);
            }
        }
    }

    private void MoveAlongPath()
    {
        if (currentPath == null || currentPath.Count == 0 || pathIndex >= currentPath.Count)
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }

        Vector2 currentPos = rb.position;
        Vector2 target = currentPath[pathIndex];
        Vector2 direction = (target - currentPos).normalized;

        float actualSpeed = isFleeingMovement ? moveSpeed * 1.5f : moveSpeed;
        rb.linearVelocity = direction * actualSpeed;

        if (Mathf.Abs(direction.x) > 0.1f)
        {
            Vector3 scale = transform.localScale;
            scale.x = direction.x > 0 ? Mathf.Abs(scale.x) : -Mathf.Abs(scale.x);
            transform.localScale = scale;
        }

        if (Vector2.Distance(currentPos, target) <= reachThreshold)
        {
            pathIndex++;
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Rigidbody2D playerRb = collision.gameObject.GetComponent<Rigidbody2D>();
            Vector2 pushDir = (collision.transform.position - transform.position).normalized;

            rb.AddForce(-pushDir * pushForce, ForceMode2D.Impulse);
            if (playerRb != null)
            {
                playerRb.AddForce(pushDir * pushForce, ForceMode2D.Impulse);
            }

            if (stateController != null)
            {
                stateController.OnCollisionWithPlayer(collision.transform);
            }
        }
    }

    private void CalculatePath(Vector2 targetPos)
    {
        if (MazeGridGraph.Instance == null) return;

        var startNode = MazeGridGraph.Instance.GetNodeFromWorld(transform.position);
        var targetNode = MazeGridGraph.Instance.GetNodeFromWorld(targetPos);

        if (startNode == null || targetNode == null) return;

        if (!targetNode.walkable)
        {
            targetNode = FindNearestWalkableNode(targetNode);
            if (targetNode == null) return;
        }

        var nodesPath = FindPath(startNode, targetNode);
        if (nodesPath == null || nodesPath.Count == 0)
        {
            currentPath.Clear();
            return;
        }

        currentPath.Clear();
        foreach (var node in nodesPath)
        {
            currentPath.Add(node.worldPosition);
        }

        pathIndex = 0;
    }

    private MazeNode FindNearestWalkableNode(MazeNode target)
    {
        Queue<MazeNode> queue = new Queue<MazeNode>();
        HashSet<MazeNode> visited = new HashSet<MazeNode>();

        queue.Enqueue(target);
        visited.Add(target);

        while (queue.Count > 0)
        {
            MazeNode current = queue.Dequeue();
            if (current.walkable) return current;

            var neighbors = MazeGridGraph.Instance.GetNeighbors(current, false);
            foreach (var neighbor in neighbors)
            {
                if (!visited.Contains(neighbor))
                {
                    visited.Add(neighbor);
                    queue.Enqueue(neighbor);
                }
            }
        }

        return null;
    }

    private List<MazeNode> FindPath(MazeNode start, MazeNode goal)
    {
        List<MazeNode> openSet = new List<MazeNode>();
        HashSet<MazeNode> closedSet = new HashSet<MazeNode>();

        start.ResetPathfinding();
        goal.ResetPathfinding();

        start.GValue = 0f;
        start.HValue = Heuristic(start, goal);
        start.CameFrom = null;

        openSet.Add(start);

        while (openSet.Count > 0)
        {
            MazeNode current = GetLowestFNode(openSet);

            if (current == goal)
            {
                return ReconstructPath(current);
            }

            openSet.Remove(current);
            closedSet.Add(current);

            var neighbors = MazeGridGraph.Instance.GetNeighbors(current, false);
            foreach (var neighbor in neighbors)
            {
                if (closedSet.Contains(neighbor)) continue;

                if (float.IsPositiveInfinity(neighbor.GValue))
                {
                    neighbor.ResetPathfinding();
                }

                float tentativeG = current.GValue + Vector2.Distance(current.worldPosition, neighbor.worldPosition);

                bool isInOpenSet = openSet.Contains(neighbor);

                if (!isInOpenSet || tentativeG < neighbor.GValue)
                {
                    neighbor.CameFrom = current;
                    neighbor.GValue = tentativeG;
                    neighbor.HValue = Heuristic(neighbor, goal);

                    if (!isInOpenSet)
                    {
                        openSet.Add(neighbor);
                    }
                }
            }
        }

        return null;
    }

    private MazeNode GetLowestFNode(List<MazeNode> nodes)
    {
        MazeNode lowest = nodes[0];
        for (int i = 1; i < nodes.Count; i++)
        {
            if (nodes[i].F() < lowest.F())
            {
                lowest = nodes[i];
            }
        }
        return lowest;
    }

    private float Heuristic(MazeNode a, MazeNode b)
    {
        return Vector2.Distance(a.worldPosition, b.worldPosition);
    }

    private List<MazeNode> ReconstructPath(MazeNode current)
    {
        List<MazeNode> path = new List<MazeNode>();
        var iter = current;

        while (iter != null)
        {
            path.Add(iter);
            iter = iter.CameFrom;
        }

        path.Reverse();
        return path;
    }

    private void OnDrawGizmos()
    {
        if (currentPath != null && currentPath.Count > 0)
        {
            Gizmos.color = isFleeingMovement ? Color.green : Color.cyan;
            for (int i = pathIndex; i < currentPath.Count - 1; i++)
            {
                Gizmos.DrawLine(currentPath[i], currentPath[i + 1]);
            }
        }
    }
}