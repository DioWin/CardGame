using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class MovementController : MonoBehaviour
{
    [SerializeField] private float speed;

    private NavMeshAgent agent;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();

        agent.speed = speed;
    }

    private Vector3? lastDestination = null;

    public void MoveTo(Vector3 destination)
    {
        if (lastDestination.HasValue && Vector3.Distance(lastDestination.Value, destination) < 0.1f)
            return;

        lastDestination = destination;
        agent.isStopped = false;
        agent.SetDestination(destination);
    }

    public void Stop()
    {
        if (agent.enabled)
            agent.isStopped = true;
    }
}