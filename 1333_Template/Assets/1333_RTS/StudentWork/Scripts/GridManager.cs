using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class GridManager : MonoBehaviour
{
    [SerializeField] private GridSettings gridSettings;
    [SerializeField] private TerrainType[] terrainTypes;

    public GridSettings GridSettings => gridSettings;

    private GridNode[,] gridNodes;
    [SerializeField] private List<GridNode> AllNodes = new();
    public bool IsInitialized { get; private set; } = false;

    public void InitializedGrid()
    {
        Debug.Log("Grid initialized!");
        gridNodes = new GridNode[gridSettings.GridSizeX, gridSettings.GridSizeY];

        for (int x = 0; x < gridSettings.GridSizeX; x++)
        {
            for (int y = 0; y < gridSettings.GridSizeY; y++)
            {
                Vector3 worldPos = gridSettings.UseXZPlane
                    ? new Vector3(x, 0, y) * gridSettings.NodeSize
                    : new Vector3(x, y, 0) * gridSettings.NodeSize;

                TerrainType terrain = terrainTypes[Random.Range(0, terrainTypes.Length)];

                GridNode node = new GridNode
                {
                    Name = $"{terrain.TerrainName}_{x}_{y}",
                    WorldPosition = worldPos,
                    terrainType = terrain,
                    walkable = terrain.Walkable,
                    Weight = terrain.MovementCost,
                    GridX = x,
                    GridY = y
                };

                AllNodes.Add(node);
                gridNodes[x, y] = node;
            }
        }
    }

    public GridNode[,] GetGrid() => gridNodes;

    public bool IsValidCoord(int x, int y)
    {
        return x >= 0 && x < gridSettings.GridSizeX && y >= 0 && y < gridSettings.GridSizeY;
    }

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