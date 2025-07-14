using System.Collections.Generic;
using UnityEngine;

public class EnemyKeeper : MonoBehaviour
{
    public static EnemyKeeper Instance { get; private set; }

    private readonly List<IEnemy> enemies = new();

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public void Register(IEnemy enemy)
    {
        if (!enemies.Contains(enemy))
            enemies.Add(enemy);
    }

    public void Unregister(IEnemy enemy)
    {
        enemies.Remove(enemy);
    }

    public IEnemy GetClosestEnemy(Vector3 position, Team team)
    {
        float minDist = float.MaxValue;
        IEnemy closest = null;

        foreach (var enemy in enemies)
        {
            if (enemy.GetTeam() == team)
                continue;

            if (!enemy.IsAlive()) continue;

            float dist = Vector3.Distance(position, enemy.GetTransform().position);
            if (dist < minDist)
            {
                minDist = dist;
                closest = enemy;
            }
        }

        return closest;
    }
}
