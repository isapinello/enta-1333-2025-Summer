using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitManager : MonoBehaviour
{
    [SerializeField] private GameObject unitPrefab;
    [SerializeField] private AStarPathfinding pathfinder;
    [SerializeField] private UnitType unitType;
    [SerializeField] private GridManager gridManager;

    private List<UnitInstance> allUnits = new();
    public List<UnitInstance> SelectedUnits { get; private set; } = new();

    void SpawnUnit(Vector3 position)
    {
        var obj = Instantiate(unitPrefab, position, Quaternion.identity);
        var unit = obj.GetComponent<UnitInstance>();
        unit.Initialize(pathfinder, unitType);
        allUnits.Add(unit);
    }

    public void SpawnUnitsNear(Vector3 bottomLeftPos, int count, int buildingSizeX, int buildingSizeY)
    {
        GridNode[,] grid = gridManager.GetGrid();
        GridNode originNode = gridManager.GetNodeFromWorldPosition(bottomLeftPos);
        if (originNode == null) return;

        int startX = originNode.GridX + buildingSizeX;
        int startY = originNode.GridY;

        int spawned = 0;
        int maxSearchRadius = 10;

        for (int r = 0; r <= maxSearchRadius && spawned < count; r++)
        {
            for (int dx = -r; dx <= r; dx++)
            {
                for (int dy = -r; dy <= r; dy++)
                {
                    int x = startX + dx;
                    int y = startY + dy;

                    if (gridManager.IsValidCoord(x, y))
                    {
                        GridNode node = grid[x, y];
                        if (node.walkable && !node.IsOccupied)
                        {
                            SpawnUnit(node.WorldPosition + Vector3.up * 0.5f);
                            node.IsOccupied = true;
                            spawned++;
                            if (spawned >= count) return;
                        }
                    }
                }
            }
        }
    }

    public void SelectUnit(UnitInstance unit, bool additive)
    {
        if (!additive)
        {
            DeselectAll();
        }

        if (!SelectedUnits.Contains(unit))
        {
            SelectedUnits.Add(unit);
            unit.OnSelect();
        }
    }

    public void DeselectAll()
    {
        foreach (var unit in SelectedUnits)
        {
            unit.OnDeselect();
        }
        SelectedUnits.Clear();
    }
}
