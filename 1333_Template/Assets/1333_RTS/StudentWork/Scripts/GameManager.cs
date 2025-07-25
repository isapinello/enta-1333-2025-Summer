using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

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
        enemyManager.StartWaves(); 
    }
    public void OnPlayerUnitKilled()
    {
        if (AreAllUnitsDead())
        {
            GoToGameOverScreen();
        }
    }

    private bool AreAllUnitsDead()
    {
        var units = FindObjectsOfType<UnitInstance>();
        return units.Length == 0;
    }

    private void GoToGameOverScreen()
    {
        // Set a static flag before switching scenes
        GameOverState.GameLost = true;
        SceneManager.LoadScene("MainMenu");
    }
}
