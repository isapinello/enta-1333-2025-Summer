using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class AStarPathfinding : MonoBehaviour
{
    [SerializeField] private GridManager gridManager;
    private GridNode[,] grid;

    public List<GridNode> FindPath(Vector3 fromWorld, Vector3 toWorld, int width = 1, int height = 1)
    {
        BuildGridReference();
        Vector2Int start = WorldToGrid(fromWorld);
        Vector2Int end = WorldToGrid(toWorld);

        if (!IsValidCoord(start) || !IsValidCoord(end)) return null;

        return AStar(start, end);
    }

    void BuildGridReference()
    {
        int width = gridManager.GridSettings.GridSizeX;
        int height = gridManager.GridSettings.GridSizeY;
        grid = new GridNode[width, height];

        foreach (var node in GetAllNodes())
        {
            string[] parts = node.Name.Split('_');
            int x = int.Parse(parts[1]);
            int y = int.Parse(parts[2]);
            grid[x, y] = node;
        }
    }

    Vector2Int WorldToGrid(Vector3 worldPos)
    {
        return new Vector2Int(Mathf.RoundToInt(worldPos.x), Mathf.RoundToInt(worldPos.z));
    }

    bool IsValidCoord(Vector2Int coord)
    {
        return coord.x >= 0 && coord.x < grid.GetLength(0) && coord.y >= 0 && coord.y < grid.GetLength(1);
    }

    List<GridNode> AStar(Vector2Int startCoord, Vector2Int endCoord)
    {
        GridNode start = grid[startCoord.x, startCoord.y];
        GridNode end = grid[endCoord.x, endCoord.y];

        var openSet = new List<GridNode> { start };
        var cameFrom = new Dictionary<GridNode, GridNode>();
        var gScore = new Dictionary<GridNode, float>();
        var fScore = new Dictionary<GridNode, float>();

        foreach (GridNode node in grid)
        {
            gScore[node] = float.MaxValue;
            fScore[node] = float.MaxValue;
        }

        gScore[start] = 0;
        fScore[start] = Heuristic(start, end);

        while (openSet.Count > 0)
        {
            GridNode current = openSet[0];
            float lowestFScore = fScore[current];

            foreach (var node in openSet)
            {
                if (fScore[node] < lowestFScore)
                {
                    lowestFScore = fScore[node];
                    current = node;
                }
            }

            openSet.Remove(current);

            if (current.Equals(end))
                return ReconstructPath(cameFrom, current);

            foreach (GridNode neighbor in GetNeighbors(current))
            {
                if (!neighbor.walkable) continue;

                float tentativeGScore = gScore[current] + neighbor.terrainType.MovementCost;

                if (tentativeGScore < gScore[neighbor])
                {
                    cameFrom[neighbor] = current;
                    gScore[neighbor] = tentativeGScore;
                    fScore[neighbor] = gScore[neighbor] + Heuristic(neighbor, end);

                    if (!openSet.Contains(neighbor))
                        openSet.Add(neighbor);
                }
            }
        }

        return new List<GridNode>(); // No path found
    }

    float Heuristic(GridNode a, GridNode b)
    {
        string[] aParts = a.Name.Split('_');
        string[] bParts = b.Name.Split('_');
        int ax = int.Parse(aParts[1]);
        int ay = int.Parse(aParts[2]);
        int bx = int.Parse(bParts[1]);
        int by = int.Parse(bParts[2]);
        return Mathf.Abs(ax - bx) + Mathf.Abs(ay - by); // Manhattan distance
    }

    List<GridNode> ReconstructPath(Dictionary<GridNode, GridNode> cameFrom, GridNode current)
    {
        List<GridNode> path = new() { current };
        while (cameFrom.ContainsKey(current))
        {
            current = cameFrom[current];
            path.Insert(0, current);
        }
        return path;
    }

    List<GridNode> GetAllNodes()
    {
        var field = typeof(GridManager).GetField("AllNodes", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        return field.GetValue(gridManager) as List<GridNode>;
    }

    List<GridNode> GetNeighbors(GridNode node)
    {
        List<GridNode> neighbors = new();
        string[] split = node.Name.Split('_');
        int x = int.Parse(split[1]);
        int y = int.Parse(split[2]);

        Vector2Int[] directions = new Vector2Int[] {
            new(0, 1), new(1, 0), new(0, -1), new(-1, 0)
        };

        foreach (Vector2Int dir in directions)
        {
            int nx = x + dir.x;
            int ny = y + dir.y;

            if (nx >= 0 && nx < grid.GetLength(0) && ny >= 0 && ny < grid.GetLength(1))
            {
                neighbors.Add(grid[nx, ny]);
            }
        }

        return neighbors;
    }
}