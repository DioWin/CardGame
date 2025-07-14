using UnityEngine;

public interface IDamageable
{
    void TakeDamage(float amount);
    bool IsAlive();
    Transform GetTransform();
    Team GetTeam();
}
