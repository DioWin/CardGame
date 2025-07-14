using UnityEngine;

public interface IEnemy
{
    Transform GetTransform();
    Team GetTeam();
    bool IsAlive();
}
