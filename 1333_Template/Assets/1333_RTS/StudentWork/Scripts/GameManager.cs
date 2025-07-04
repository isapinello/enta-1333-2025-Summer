using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private GridManager gridManager;
    [SerializeField] private UnitManager unitManager;
    [SerializeField] private BuildingPlacing buildingPlacing;
    [SerializeField] private EnemyManager enemyManager;
    [SerializeField] private GameObject enemyPrefab;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            enemyManager.SpawnRandomEnemies(5, enemyPrefab, gridManager);
        }
    }
    private void Awake()
    {
        gridManager.InitializedGrid();
        enemyManager.InitializeEnemyUnits();
    }

    public void StartPlacingBuilding(BuildingData buildingData)
    {
        buildingPlacing.BeginPlacing(buildingData);
    }

    public void NotifyFirstBuildingPlaced()
    {
        enemyManager.ActivateEnemyUnits();
    }
}
