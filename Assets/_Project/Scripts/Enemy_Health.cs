using System;
using UnityEngine;

public class Enemy_Health : MonoBehaviour, IDamageable
{
    public event Action<Enemy_Health> OnEnemyDied;

    [Header("Health")]
    [SerializeField] private float maxHealth = 100f;

    private float currentHealth;
    private bool isDead;

    private void Awake()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(float amount)
    {
        if (isDead)
            return;

        currentHealth -= amount;
        Debug.Log($"{gameObject.name} took {amount} damage. HP: {currentHealth}");

        if (currentHealth <= 0f)
        {
            Die();
        }
    }

    private void Die()
    {
        if (isDead)
            return;

        isDead = true;

        Debug.Log($"{gameObject.name} died.");

        OnEnemyDied?.Invoke(this);

        Destroy(gameObject);
    }
}