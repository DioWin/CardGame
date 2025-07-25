using UnityEngine;
using UnityEngine.AI;

public class AIBehaviour : MonoBehaviour
{
    public enum State { Idle, Move, Attack }

    [SerializeField] private float detectionRange = 10f;
    [SerializeField] private float attackRange = 2f;
    [SerializeField] private Team myTeam = Team.Enemy;

    public State currentState = State.Idle;
    private IDamageable target;
    private MovementController movement;
    private WeaponControlSystem weapon;
    private HealthController healthController;
    private NavMeshAgent navMeshAgent;

    private bool isBlockedBehavior;

    private void Awake()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        movement = GetComponent<MovementController>();
        weapon = GetComponent<WeaponControlSystem>();
        healthController = GetComponent<HealthController>();

        healthController.OnDeath += HandleDeath;
    }

    private void OnDestroy()
    {
        healthController.OnDeath -= HandleDeath;
    }

    private void HandleDeath()
    {
        movement.enabled = false;
        navMeshAgent.enabled = false;

        movement.Stop();
        isBlockedBehavior = true;
    }

    private void Update()
    {
        if (isBlockedBehavior)
            return;

        UpdateState();
        HandleState();
    }

    private void UpdateState()
    {
        if (target == null || !target.IsAlive())
        {
            target = FindClosestTarget();

            currentState = State.Idle;
            return;
        }

        float dist = Vector3.Distance(transform.position, target.GetTransform().position);

        if (dist <= attackRange)
            currentState = State.Attack;
        else if (dist <= detectionRange)
            currentState = State.Move;
        else
            currentState = State.Idle;
    }

    private void HandleState()
    {

        switch (currentState)
        {
            case State.Idle:
                movement.Stop();
                break;
            case State.Move:
                movement.MoveTo(target.GetTransform().position);
                break;
            case State.Attack:
                movement.Stop();
                weapon.TryAttack(target);
                break;
        }
    }

    private IDamageable FindClosestTarget()
    {
        var enemies = EnemyKeeper.Instance;
        IEnemy closest = enemies.GetClosestEnemy(transform.position, myTeam);

        if (closest == null) return null;

        var dmg = closest as IDamageable;
        if (dmg == null || dmg.GetTeam() == myTeam)
            return null;

        return dmg;
    }
}
