using UnityEngine;

public class TowerController : MonoBehaviour, IDamageable, IEnemy
{
    [SerializeField] private Team team = Team.Enemy;
    private HealthController health;

    private void Start()
    {
        health = GetComponent<HealthController>();

        EnemyKeeper.Instance.Register(this);
    }

    public Transform GetTransform() => transform;
    public bool IsAlive() => health != null && health.IsAlive;
    public Team GetTeam() => team;

    public void TakeDamage(float amount)
    {
        if (IsAlive())
            health.TakeDamage(amount);
    }
}
