using UnityEngine;

public class WeaponControlSystem : MonoBehaviour
{
    [SerializeField] private float attackCooldown = 1f;
    [SerializeField] private float damage = 10f;

    private float lastAttackTime;

    public void TryAttack(IDamageable target)
    {
        if (Time.time - lastAttackTime < attackCooldown) return;

        if (target != null && target.IsAlive())
        {
            target.TakeDamage(damage);
            lastAttackTime = Time.time;
        }
    }
}
