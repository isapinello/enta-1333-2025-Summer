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
}