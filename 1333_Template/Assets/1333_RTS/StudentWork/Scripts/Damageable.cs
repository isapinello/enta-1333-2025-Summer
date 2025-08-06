using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Damageable : MonoBehaviour
{
    [SerializeField] private int maxHealth = 100;
    private int currentHealth;

    private HealthBar bar;

    private void Awake()
    {
        currentHealth = maxHealth;
        bar = GetComponentInChildren<HealthBar>(true); // search children
        UpdateBar();
    }

    public void TakeDamage(int amt)
    {
        currentHealth = Mathf.Max(0, currentHealth - amt);
        UpdateBar();

        if (currentHealth <= 0)
            Destroy(gameObject);          // bar dies with its parent
    }

    private void UpdateBar()
    {
        if (bar != null)
            bar.SetRatio((float)currentHealth / maxHealth);
    }
}
