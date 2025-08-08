using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Damageable : MonoBehaviour
{
    [SerializeField] private int maxHealth = 100;
    private int currentHealth;
    private bool isDead;

    private HealthBar bar;

    private void Awake()
    {
        currentHealth = maxHealth;
        bar = GetComponentInChildren<HealthBar>(true); // search children
        UpdateBar();
    }

    public void TakeDamage(int amount)
    {
        if (isDead) return;

        currentHealth = Mathf.Max(0, currentHealth - amount);
        UpdateBar();

        if (currentHealth <= 0)
        {
            isDead = true;
            Destroy(gameObject); // UnitManager gets notified via UnitInstance.OnDestroy()
        }
    }

    private void UpdateBar()
    {
        if (bar != null)
            bar.SetRatio((float)currentHealth / maxHealth);
    }
}
