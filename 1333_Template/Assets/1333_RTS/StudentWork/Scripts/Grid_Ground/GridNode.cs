using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Represents each node on our grid. Brutally efficient
[System.Serializable]
public class GridNode
{
    public string Name; // Grid Index
    public Vector3 WorldPosition;
    public bool walkable;
    public int Weight;
    public TerrainType terrainType;

    public int GridX;
    public int GridY;
    public Color GizmoColor => terrainType != null
                                ? terrainType.GizmoColor
                                : Color.white;

    public bool IsOccupied = false;
}

