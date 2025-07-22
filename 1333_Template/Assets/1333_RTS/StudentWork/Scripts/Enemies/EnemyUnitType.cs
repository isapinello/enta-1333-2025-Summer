using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EnemyUnitType", menuName = "Game/Enemy Unit Type")]
public class EnemyUnitType : ScriptableObject
{
    public string unitName;
    public float moveSpeed = 2f;
    public float attackRange = 1.5f;
    public float health = 80f;
    public float attackDamage = 10f;
    public Material enemyMaterial;
}
