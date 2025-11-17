using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MazeGridGraph : MonoBehaviour
{
    public static MazeGridGraph Instance { get; private set; }

    [Header("Tilemap References")]
    [SerializeField] private Tilemap collisionTilemap;

    [Header("Grid Settings")]
    [SerializeField] private float cellSize = 1f;
    [SerializeField] private Vector2 gridOffset = Vector2.zero;

    private Dictionary<Vector2Int, MazeNode> nodes = new Dictionary<Vector2Int, MazeNode>();
    private BoundsInt gridBounds;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        BuildGraph();
    }

    private void BuildGraph()
    {
        if (collisionTilemap == null)
        {
            return;
        }

        collisionTilemap.CompressBounds();
        gridBounds = collisionTilemap.cellBounds;

        for (int x = gridBounds.xMin; x < gridBounds.xMax; x++)
        {
            for (int y = gridBounds.yMin; y < gridBounds.yMax; y++)
            {
                Vector3Int cellPos = new Vector3Int(x, y, 0);
                Vector2 worldPos = collisionTilemap.CellToWorld(cellPos) + new Vector3(cellSize / 2, cellSize / 2, 0);

                bool isWalkable = !collisionTilemap.HasTile(cellPos);

                MazeNode node = new MazeNode(x, y, worldPos, isWalkable);
                nodes[new Vector2Int(x, y)] = node;
            }
        }
    }

    public MazeNode GetNode(int x, int y)
    {
        nodes.TryGetValue(new Vector2Int(x, y), out var node);
        return node;
    }

    public MazeNode GetNodeFromWorld(Vector2 worldPosition)
    {
        if (collisionTilemap == null) return null;

        Vector3Int cellPos = collisionTilemap.WorldToCell(worldPosition);
        return GetNode(cellPos.x, cellPos.y);
    }

    public List<MazeNode> GetNeighbors(MazeNode node, bool allowDiagonal = false)
    {
        var result = new List<MazeNode>();
        if (node == null) return result;

        int[,] directions = allowDiagonal
            ? new int[,] { { 0, 1 }, { 1, 0 }, { 0, -1 }, { -1, 0 }, { 1, 1 }, { 1, -1 }, { -1, 1 }, { -1, -1 } }
            : new int[,] { { 0, 1 }, { 1, 0 }, { 0, -1 }, { -1, 0 } };

        for (int i = 0; i < directions.GetLength(0); i++)
        {
            int nx = node.x + directions[i, 0];
            int ny = node.y + directions[i, 1];

            var neighbor = GetNode(nx, ny);
            if (neighbor == null || !neighbor.walkable) continue;

            if (allowDiagonal && Mathf.Abs(directions[i, 0]) + Mathf.Abs(directions[i, 1]) == 2)
            {
                var n1 = GetNode(node.x + directions[i, 0], node.y);
                var n2 = GetNode(node.x, node.y + directions[i, 1]);
                if (n1 == null || n2 == null || !n1.walkable || !n2.walkable)
                    continue;
            }

            result.Add(neighbor);
        }

        return result;
    }

    public Vector2 GetRandomWalkablePosition()
    {
        List<MazeNode> walkableNodes = new List<MazeNode>();
        foreach (var node in nodes.Values)
        {
            if (node.walkable)
                walkableNodes.Add(node);
        }

        if (walkableNodes.Count == 0)
            return Vector2.zero;

        return walkableNodes[Random.Range(0, walkableNodes.Count)].worldPosition;
    }

    private void OnDrawGizmos()
    {
        if (nodes == null || nodes.Count == 0) return;

        foreach (var node in nodes.Values)
        {
            Gizmos.color = node.walkable ? new Color(0, 1, 0, 0.3f) : new Color(1, 0, 0, 0.3f);
            Gizmos.DrawCube(node.worldPosition, Vector3.one * cellSize * 0.8f);
        }
    }
}

public class MazeNode
{
    public int x;
    public int y;
    public Vector2 worldPosition;
    public bool walkable;

    public float GValue;
    public float HValue;
    public MazeNode CameFrom;

    public MazeNode(int x, int y, Vector2 worldPosition, bool walkable)
    {
        this.x = x;
        this.y = y;
        this.worldPosition = worldPosition;
        this.walkable = walkable;
        ResetPathfinding();
    }

    public float F() => GValue + HValue;

    public void ResetPathfinding()
    {
        GValue = float.PositiveInfinity;
        HValue = 0f;
        CameFrom = null;
    }
}