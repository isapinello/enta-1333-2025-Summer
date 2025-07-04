using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    [SerializeField] private List<EnemyUnit> enemyUnits;
    [SerializeField] private UnitManager unitManager;
    [SerializeField] private List<BuildingInstance> playerBuildings;
    [SerializeField] private AStarPathfinding pathfinder;

    private bool active = false;

    public void InitializeEnemyUnits()
    {
        foreach (var unit in enemyUnits)
        {
            unit.SetPathfinder(pathfinder);
            unit.SetActive(false);
        }
    }

    public void ActivateEnemyUnits()
    {
        active = true;
        foreach (var unit in enemyUnits)
        {
            unit.SetActive(true);
            unit.StartBehavior();
        }
    }

    public List<BuildingInstance> GetPlayerBuildings() => playerBuildings;
    public List<UnitInstance> GetPlayerUnits() => unitManager.GetAllPlayerUnits();

    public void RegisterPlayerBuilding(BuildingInstance building)
    {
        if (!playerBuildings.Contains(building))
            playerBuildings.Add(building);
    }
    public void SpawnRandomEnemies(int count, GameObject enemyPrefab, GridManager gridManager)
    {
        GridNode[,] grid = gridManager.GetGrid();
        int gridSizeX = grid.GetLength(0);
        int gridSizeY = grid.GetLength(1);
        int spawned = 0;
        int attempts = 0;
        int maxAttempts = 500;

        while (spawned < count && attempts < maxAttempts)
        {
            int x = Random.Range(0, gridSizeX);
            int y = Random.Range(0, gridSizeY);
            GridNode node = grid[x, y];

            if (node.walkable && !node.IsOccupied)
            {
                Vector3 spawnPos = node.WorldPosition + Vector3.up * 0.5f;
                GameObject obj = Instantiate(enemyPrefab, spawnPos, Quaternion.identity);
                EnemyUnit enemy = obj.GetComponent<EnemyUnit>();
                enemy.SetPathfinder(pathfinder);
                enemy.SetActive(true);
                enemy.StartBehavior();

                node.IsOccupied = true;
                enemyUnits.Add(enemy);
                spawned++;
            }

            attempts++;
        }
    }
}
