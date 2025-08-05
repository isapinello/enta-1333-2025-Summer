using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Damageable : MonoBehaviour
{
    [SerializeField] private int maxHealth = 100;
    private int currentHealth;

    private GameObject healthBarPrefab;
    private HealthBar healthBar;

    public void SetHealthBarPrefab(GameObject prefab)
    {
        healthBarPrefab = prefab;
    }

    private void Start()
    {
        currentHealth = maxHealth;

        if (healthBarPrefab != null)
        {
            GameObject hb = Instantiate(healthBarPrefab, transform.position, Quaternion.identity);
            healthBar = hb.GetComponent<HealthBar>();
            healthBar.AttachTo(transform);
            UpdateHealthBar();
        }
    }

    public void TakeDamage(int amount)
    {
        currentHealth -= amount;
        if (currentHealth < 0) currentHealth = 0;

        UpdateHealthBar();

        if (currentHealth <= 0)
        {
            if (healthBar != null)
                Destroy(healthBar.gameObject);
            Destroy(gameObject);
        }
    }

    private void UpdateHealthBar()
    {
        if (healthBar != null)
        {
            float normalizedHealth = (float)currentHealth / maxHealth;
            healthBar.SetValue(normalizedHealth);
        }
    }
}
