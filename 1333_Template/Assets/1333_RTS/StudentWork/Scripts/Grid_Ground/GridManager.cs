using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class GridManager : MonoBehaviour
{
    [SerializeField] private GridSettings gridSettings; // Settings for grid size, spacing, and dimensions
    [SerializeField] private TerrainType[] terrainTypes; // List of possible terrain types for nodes

    public GridSettings GridSettings => gridSettings; // Public getter for other scripts

    private GridNode[,] gridNodes; // 2D array that holds all nodes in the grid
    [SerializeField] private List<GridNode> AllNodes = new(); // Flat list of all grid nodes for reference/debugging

    public bool IsInitialized { get; private set; } = false; // Tracks if the grid has been initialized

    // === Initializes the full grid ===
    public void InitializedGrid()
    {
        Debug.Log("Grid initialized!");

        gridNodes = new GridNode[gridSettings.GridSizeX, gridSettings.GridSizeY];

        for (int x = 0; x < gridSettings.GridSizeX; x++)
        {
            for (int y = 0; y < gridSettings.GridSizeY; y++)
            {
                // Compute the world position based on the grid layout (2D or 3D)
                Vector3 worldPos = gridSettings.UseXZPlane
                    ? new Vector3(x, 0, y) * gridSettings.NodeSize  // 3D layout (top-down)
                    : new Vector3(x, y, 0) * gridSettings.NodeSize; // 2D layout (side view)

                // Pick a random terrain type for this tile
                TerrainType terrain = terrainTypes[Random.Range(0, terrainTypes.Length)];

                // Create the grid node with coordinates and terrain properties
                GridNode node = new GridNode
                {
                    Name = $"{terrain.TerrainName}_{x}_{y}",
                    WorldPosition = worldPos,
                    terrainType = terrain,
                    walkable = terrain.Walkable,
                    Weight = terrain.MovementCost,
                    GridX = x, // Store grid X coordinate
                    GridY = y  // Store grid Y coordinate
                };

                AllNodes.Add(node);        // Add to full list
                gridNodes[x, y] = node;    // Assign to grid array
            }
        }

        IsInitialized = true;
    }

    // === Returns the full 2D node grid ===
    public GridNode[,] GetGrid() => gridNodes;

    // === Checks whether a given (x, y) is inside the grid bounds ===
    public bool IsValidCoord(int x, int y)
    {
        return x >= 0 && x < gridSettings.GridSizeX &&
               y >= 0 && y < gridSettings.GridSizeY;
    }

    // === Finds a node based on a world position in the scene ===
    public GridNode GetNodeFromWorldPosition(Vector3 worldPos)
    {
        int x = Mathf.FloorToInt(worldPos.x);
        int y = Mathf.FloorToInt(worldPos.z);

        if (IsValidCoord(x, y))
        {
            return gridNodes[x, y];
        }
        return null;
    }

    // === Draws wireframe cubes for each grid tile in the Scene view ===
    private void OnDrawGizmos()
    {
        if (gridNodes == null || gridSettings == null) return;

        for (int x = 0; x < gridSettings.GridSizeX; x++)
        {
            for (int y = 0; y < gridSettings.GridSizeY; y++)
            {
                GridNode node = gridNodes[x, y];
                Gizmos.color = node.walkable ? node.GizmoColor : Color.red;
                Gizmos.DrawWireCube(node.WorldPosition, Vector3.one * gridSettings.NodeSize * 0.9f);
            }
        }
    }
}