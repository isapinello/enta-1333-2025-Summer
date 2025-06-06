using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class UnitSelector : MonoBehaviour
{
    private Camera cam;
    [SerializeField] private UnitManager unitManager;
    [SerializeField] private GridManager gridManager; 
    [SerializeField] private AStarPathfinding pathfinder;
    [SerializeField] private LayerMask groundLayer;

    private UnitInstance selectedUnit;

    void Start()
    {
        cam = Camera.main;
    }

    void Update()
    {
        HandleSelection();
        HandleRightClickCommand();
    }

    void HandleSelection()
    {
        if (Input.GetMouseButtonDown(0)) // Left click
        {
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                var unit = hit.collider.GetComponent<UnitInstance>();
                bool additive = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);

                if (unit != null)
                {
                    unitManager.SelectUnit(unit, additive);
                }
                else if (!additive)
                {
                    unitManager.DeselectAll(); // Only clear if not additive
                }
            }
            else
            {
                unitManager.DeselectAll();
            }
        }
    }

    void HandleRightClickCommand()
    {
        if (Input.GetMouseButtonDown(1) && unitManager.SelectedUnits.Count > 0)
        {
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, 1000, groundLayer))
            {
                Vector2Int gridCoord = new Vector2Int(
                    Mathf.RoundToInt(hit.point.x),
                    Mathf.RoundToInt(hit.point.z)
                );

                GridNode[,] grid = gridManager.GetGrid();
                if (gridCoord.x < 0 || gridCoord.x >= grid.GetLength(0) || gridCoord.y < 0 || gridCoord.y >= grid.GetLength(1))
                    return;

                GridNode targetNode = grid[gridCoord.x, gridCoord.y];
                if (!targetNode.walkable) return;

                // Move all selected units
                foreach (var unit in unitManager.SelectedUnits)
                {
                    unit.MoveTo(targetNode);
                }
            }
        }
    }
}
