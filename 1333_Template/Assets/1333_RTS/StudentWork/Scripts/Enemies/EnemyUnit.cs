using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyUnit : MonoBehaviour
{
    [SerializeField] private float speed = 2f;
    [SerializeField] private float attackRange = 1.5f;
    [SerializeField] private float attackCooldown = 1.0f;
    [SerializeField] private int damage = 10;

    private AStarPathfinding pathfinder;
    private List<GridNode> _currentPath = new();
    private int _pathIndex = 0;
    private GameObject currentTarget;
    private Coroutine behaviorRoutine;
    private bool active = false;

    private Damageable damageable;   // just a reference

    private void Awake() => damageable = GetComponent<Damageable>();

    public void SetPathfinder(AStarPathfinding p) => pathfinder = p;
    public void SetActive(bool state) => active = state;

    public void StartBehavior()
    {
        if (behaviorRoutine != null) StopCoroutine(behaviorRoutine);
        behaviorRoutine = StartCoroutine(BehaviorLoop());
    }

    IEnumerator BehaviorLoop()
    {
        while (active)
        {
            GameObject target = FindClosestTarget();
            if (target != null)
            {
                float dist = Vector3.Distance(transform.position, target.transform.position);

                if (target != currentTarget || _pathIndex >= _currentPath.Count)
                {
                    currentTarget = target;
                    SetTarget(currentTarget.transform.position);
                }

                if (dist <= attackRange)
                {
                    target.GetComponent<Damageable>()?.TakeDamage(damage);
                    yield return new WaitForSeconds(attackCooldown);
                }
            }
            yield return null;
        }
    }

    void SetTarget(Vector3 pos)
    {
        _currentPath = pathfinder.FindPath(transform.position, pos, 1, 1);
        _pathIndex = 0;
    }

    private void Update()
    {
        if (!active || _currentPath == null || _pathIndex >= _currentPath.Count) return;

        Vector3 next = _currentPath[_pathIndex].WorldPosition;
        transform.position = Vector3.MoveTowards(transform.position, next, speed * Time.deltaTime);

        if (Vector3.Distance(transform.position, next) < 0.05f) _pathIndex++;
    }


    GameObject FindClosestTarget()
    {
        var buildings = FindObjectsOfType<BuildingInstance>();
        GameObject closest = null;
        float minDist = float.MaxValue;

        foreach (var b in buildings)
        {
            float d = Vector3.Distance(transform.position, b.transform.position);
            if (d < minDist)
            {
                minDist = d;
                closest = b.gameObject;
            }
        }

        if (closest != null) return closest;

        var units = FindObjectsOfType<UnitInstance>();
        foreach (var u in units)
        {
            float d = Vector3.Distance(transform.position, u.transform.position);
            if (d < minDist)
            {
                minDist = d;
                closest = u.gameObject;
            }
        }

        return closest;
    }
}