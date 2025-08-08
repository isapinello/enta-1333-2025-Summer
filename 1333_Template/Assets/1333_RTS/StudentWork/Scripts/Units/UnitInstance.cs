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

    private float attackRange = 1.5f;
    private float attackCooldown = 1.0f;
    private int damage = 20;
    private GameObject currentTarget;
    private bool active = true;

    private Renderer cachedRenderer;
    private Coroutine behaviorRoutine;

    // --- NEW: owner reference so we can notify the manager on death ---
    private UnitManager _ownerManager;
    public void SetOwner(UnitManager owner) => _ownerManager = owner;

    public bool IsMoving => _isMoving;

    private void Start()
    {
        StartBehavior(); // no Damageable add here
    }

    public void Initialize(AStarPathfinding pathfinder, UnitType unitType)
    {
        _pathfinder = pathfinder;
        moveSpeed = unitType.moveSpeed;
        attackRange = unitType.attackRange;
        damage = Mathf.RoundToInt(unitType.attackDamage);
        cachedRenderer = GetComponentInChildren<Renderer>();

        if (cachedRenderer != null)
            cachedRenderer.material = unitType.teamMaterial;
    }

    private void Update()
    {
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
        AudioManager.Instance.PlayUnitSelectSound();
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

    private GameObject FindClosestEnemy()
    {
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
    private void OnDestroy()
    {
        _ownerManager?.OnUnitDestroyed(this);
    }
}
