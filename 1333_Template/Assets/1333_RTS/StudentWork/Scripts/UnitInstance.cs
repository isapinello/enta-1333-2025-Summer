using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitInstance : UnitBase, ISelectable
{
    [Header("Movement")]
    private float moveSpeed;

    private AStarPathfinding _pathfinder; // Reference to the pathfinding system
    private List<GridNode> _currentPath = new(); // The current path this unit will follow
    private int _pathIndex = 0; // Index of the current node in the path
    private bool _isMoving = false; // Flag to check if the unit is moving
    private float attackRange = 1.5f;
    private float attackCooldown = 1.0f;
    private int damage = 20;
    private GameObject currentTarget;
    private bool active = true;

    private Renderer cachedRenderer; // Cached reference to the unit's renderer
    private Coroutine behaviorRoutine;
    public bool IsMoving => _isMoving;

    private void Start()
    {
        gameObject.AddComponent<Damageable>();
        StartBehavior();
    }
    public void Initialize(AStarPathfinding pathfinder, UnitType unitType)
    {
        _pathfinder = pathfinder;
        moveSpeed = unitType.moveSpeed;
        attackRange = unitType.attackRange;
        damage = Mathf.RoundToInt(unitType.attackDamage);
        cachedRenderer = GetComponentInChildren<Renderer>();
        gameObject.AddComponent<Damageable>();

        if (cachedRenderer != null)
            cachedRenderer.material = unitType.teamMaterial;
    }

    private void Update()
    {
        /*if (!_isMoving || _currentPath == null || _currentPath.Count == 0 || _pathIndex >= _currentPath.Count)
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
        }*/
        if (_currentPath == null || _currentPath.Count == 0 || _pathIndex >= _currentPath.Count)
            return;

        Vector3 nextWaypoint = _currentPath[_pathIndex].WorldPosition;
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
    public void StartBehavior()
    {
        if (behaviorRoutine != null) StopCoroutine(behaviorRoutine);
        behaviorRoutine = StartCoroutine(BehaviorLoop());
    }
    IEnumerator BehaviorLoop()
    {
        while (active)
        {
            GameObject target = FindClosestEnemy();
            if (target != null)
            {
                float distance = Vector3.Distance(transform.position, target.transform.position);

                if (target != currentTarget || _pathIndex >= _currentPath.Count)
                {
                    currentTarget = target;
                    SetTarget(currentTarget.transform.position);
                    _isMoving = true;
                }

                if (distance <= attackRange)
                {
                    target.GetComponent<Damageable>()?.TakeDamage(damage);
                    yield return new WaitForSeconds(attackCooldown);
                }
            }

            yield return null;
        }
    }
    /*public void SetTarget(Vector3 worldPosition)
    {
        _currentPath = _pathfinder.FindPath(transform.position, worldPosition, Width, Height);
        _pathIndex = 0;
        _isMoving = _currentPath != null && _currentPath.Count > 1;
    }*/
    void SetTarget(Vector3 targetPos)
    {
        _currentPath = _pathfinder.FindPath(transform.position, targetPos, 1, 1);
        _pathIndex = 0;
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
            Gizmos.DrawLine(
                _currentPath[i].WorldPosition + Vector3.up * 0.2f,
                _currentPath[i + 1].WorldPosition + Vector3.up * 0.2f
            );
        }
    }
    private IEnumerator CombatBehavior()
    {
        while (true)
        {
            GameObject target = FindClosestEnemy();
            if (target != null)
            {
                float distance = Vector3.Distance(transform.position, target.transform.position);
                if (distance <= 1.5f)
                {
                    target.GetComponent<Damageable>()?.TakeDamage(20);
                    yield return new WaitForSeconds(1f);
                }
                else
                {
                    transform.position = Vector3.MoveTowards(transform.position, target.transform.position, moveSpeed * Time.deltaTime);
                }
            }
            yield return null;
        }
    }
    private GameObject FindClosestEnemy()
    {
        /*GameObject[] enemies = GameObject.FindGameObjectsWithTag("EnemyUnit");
        float closestDistance = Mathf.Infinity;
        GameObject closest = null;

        foreach (var enemy in enemies)
        {
            float d = Vector3.Distance(transform.position, enemy.transform.position);
            if (d < closestDistance)
            {
                closestDistance = d;
                closest = enemy;
            }
        }

        return closest;*/
        var enemies = GameObject.FindGameObjectsWithTag("EnemyUnit");
        GameObject closest = null;
        float minDist = float.MaxValue;

        foreach (var e in enemies)
        {
            float d = Vector3.Distance(transform.position, e.transform.position);
            if (d < minDist)
            {
                minDist = d;
                closest = e;
            }
        }

        return closest;
    }
}
