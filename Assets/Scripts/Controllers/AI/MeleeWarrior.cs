using UnityEngine;

[RequireComponent(typeof(HealthController))]
[RequireComponent(typeof(AIBehaviour))]
[RequireComponent(typeof(MovementController))]
[RequireComponent(typeof(WeaponControlSystem))]
public class MeleeWarrior : MonoBehaviour, IEnemy, IDamageable
{
    [SerializeField] private Team team = Team.Enemy;
    private HealthController health;

    private void Awake()
    {
        health = GetComponent<HealthController>();
        EnemyKeeper.Instance.Register(this);
    }

    private void OnDestroy()
    {
        if (EnemyKeeper.Instance != null)
            EnemyKeeper.Instance.Unregister(this);
    }

    public Transform GetTransform()
    {
        if (this == null) 
            return null;

        return this.transform;
    }

    public bool IsAlive() => health != null && health.IsAlive;
    public Team GetTeam() => team;

    public void TakeDamage(float amount)
    {
        if (IsAlive())
            health.TakeDamage(amount);
    }
}
