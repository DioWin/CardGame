using System;
using Unity.Collections;
using UnityEngine;

public class HealthController : MonoBehaviour
{
    public Action OnDeath;
    public Action OnTakeDamage;

    [SerializeField] private float maxHealth = 100;
    [SerializeField][ReadOnly] private float currentHealth;
    [SerializeField] private bool CanDestroyOnDeath = false;

    public bool IsAlive => currentHealth > 0;
    public bool isImortal;

    private void Awake()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(float amount)
    {
        if (isImortal)
            return;

        if (!IsAlive) return;

        OnTakeDamage?.Invoke();

        currentHealth -= amount;
        if (currentHealth <= 0)
            Death();
    }

    private void Death()
    {
        // TODO: Add death logic

        OnDeath?.Invoke();

        if (CanDestroyOnDeath)
            Destroy(gameObject, 2f);
    }
}
