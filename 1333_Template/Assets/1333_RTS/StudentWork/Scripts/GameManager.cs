using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private GridManager gridManager;
    [SerializeField] private UnitManager unitManager;
    [SerializeField] private BuildingPlacing buildingPlacing;

    private void Awake()
    {
        gridManager.InitializedGrid();
    }

    public void StartPlacingBuilding(BuildingData buildingData)
    {
        buildingPlacing.BeginPlacing(buildingData);
    }
}
