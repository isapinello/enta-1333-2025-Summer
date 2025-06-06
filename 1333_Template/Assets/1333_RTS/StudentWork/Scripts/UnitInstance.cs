using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitInstance : UnitBase, ISelectable
{
    [Header("Movement")]
    private float moveSpeed;

    private AStarPathfinding _pathfinder;
    private List<GridNode> _currentPath = new();
    private int _pathIndex = 0;
    private bool _isMoving = false;

    private Renderer cachedRenderer;

    public bool IsMoving => _isMoving;

    public void Initialize(AStarPathfinding pathfinder, UnitType unitType)
    {
        _pathfinder = pathfinder;
        _unitType = unitType;
        moveSpeed = unitType.moveSpeed;
        cachedRenderer = GetComponentInChildren<Renderer>();
        if (cachedRenderer != null)
            cachedRenderer.material = unitType.teamMaterial;
    }

    private void Update()
    {
        if (!_isMoving || _currentPath == null || _currentPath.Count == 0 || _pathIndex >= _currentPath.Count)
            return;

        Vector3 nextWaypoint = _currentPath[_pathIndex].WorldPosition;
        Vector3 direction = (nextWaypoint - transform.position).normalized;
        float step = moveSpeed * Time.deltaTime;
        transform.position = Vector3.MoveTowards(transform.position, nextWaypoint, step);

        if (Vector3.Distance(transform.position, nextWaypoint) < 0.05f)
        {
            _pathIndex++;
            if (_pathIndex >= _currentPath.Count)
            {
                _isMoving = false;
            }
        }
    }

    public void SetTarget(Vector3 worldPosition)
    {
        _currentPath = _pathfinder.FindPath(transform.position, worldPosition, Width, Height);
        _pathIndex = 0;
        _isMoving = _currentPath != null && _currentPath.Count > 1;
    }

    public void SetTarget(GridNode node)
    {
        SetTarget(node.WorldPosition);
    }

    public override void MoveTo(GridNode targetNode)
    {
        SetTarget(targetNode);
    }

    public void OnSelect()
    {
        if (cachedRenderer != null)
            cachedRenderer.material.color = Color.cyan;
    }

    public void OnDeselect()
    {
        if (cachedRenderer != null && _unitType != null && _unitType.teamMaterial != null)
            cachedRenderer.material.color = _unitType.teamMaterial.color;
    }

    public string GetLabel()
    {
        return _unitType.name;
    }
    private void OnDrawGizmos()
    {
        if (_currentPath == null || _currentPath.Count < 2) return;

        Gizmos.color = Color.yellow;
        for (int i = 0; i < _currentPath.Count - 1; i++)
        {
            Gizmos.DrawLine(_currentPath[i].WorldPosition + Vector3.up * 0.2f,
                            _currentPath[i + 1].WorldPosition + Vector3.up * 0.2f);
        }
    }
}
