using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="BuildingTypeSo", menuName = "ScriptableObjects/BuildingTypes")]
public class BuildingTypeSo : ScriptableObject
{
    public List<BuildingData> Buildings = new (); 
}
[System.Serializable]

public class BuildingData
{
    public string BuildingId;
    public GameObject buildingPrefabs;
    public int gridSizeX = 1;
    public int gridSizeY = 1;

    public int gridOffsetX = 1;
    public int gridOffsetY = 1;

    public bool canSpawnUnits = false;
}