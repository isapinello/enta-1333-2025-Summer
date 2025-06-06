using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitManager : MonoBehaviour
{
    [SerializeField] private GameObject unitPrefab;
    [SerializeField] private AStarPathfinding pathfinder;
    [SerializeField] private UnitType unitType;

    private List<UnitInstance> allUnits = new();
    public List<UnitInstance> SelectedUnits { get; private set; } = new();

    public void SpawnDemoUnits(GridNode[,] grid, GridSettings settings)
    {
        int spawned = 0;
        int targetCount = 5;

        while (spawned < targetCount)
        {
            int x = Random.Range(0, settings.GridSizeX);
            int y = Random.Range(0, settings.GridSizeY);
            GridNode node = grid[x, y];

            if (!node.walkable) continue;

            Vector3 spawnPos = node.WorldPosition + Vector3.up * 0.5f;
            SpawnUnit(spawnPos);
            spawned++;
        }
    }

    public void SpawnUnit(Vector3 position)
    {
        var obj = Instantiate(unitPrefab, position, Quaternion.identity);
        var unit = obj.GetComponent<UnitInstance>();
        unit.Initialize(pathfinder, unitType);
        allUnits.Add(unit);
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

    public void CommandSelectedUnitsToMove(GridNode targetNode)
    {
        foreach (var unit in SelectedUnits)
        {
            unit.MoveTo(targetNode);
        }
    }
}
