using Unity.Collections;
using UnityEngine;

public class HealthController : MonoBehaviour
{
    [SerializeField] private float maxHealth = 100;
    [SerializeField][ReadOnly] private float currentHealth;

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

        currentHealth -= amount;
        if (currentHealth <= 0)
            Die();
    }

    private void Die()
    {
        // TODO: Add death logic
        Destroy(gameObject);
    }
}
