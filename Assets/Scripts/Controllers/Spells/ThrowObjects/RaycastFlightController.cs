using UnityEngine;

public class RaycastFlightController : MonoBehaviour
{
    [SerializeField] private float speed = 10f;
    [SerializeField] private float arcHeight = 2f;
    [SerializeField] private float arrivalThreshold = 0.2f;
    public float randomRadius = 1f;

    private Vector3 start;
    private Vector3 target;
    private float progress;
    private bool initialized;

    public bool HasArrived { get; private set; }

    public void Init(Vector3 start, Vector3 target, float speed, float arcHeight)
    {
        this.start = start;
        this.target = target;
        this.speed = speed;
        this.arcHeight = arcHeight;
        transform.position = start;
        progress = 0f;
        initialized = true;
    }

    private void Update()
    {
        if (!initialized || HasArrived) return;

        float totalDistance = Vector3.Distance(start, target);
        progress += Time.deltaTime * speed / totalDistance;
        progress = Mathf.Clamp01(progress);

        Vector3 nextPos = Vector3.Lerp(start, target, progress);
        nextPos.y += Mathf.Sin(progress * Mathf.PI) * arcHeight;
        transform.position = nextPos;

        if (Vector3.Distance(transform.position, target) < arrivalThreshold || progress >= 1f)
            HasArrived = true;
    }

    public Vector3 GetRaycastHitPoint()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Vector3 baseTarget = Physics.Raycast(ray, out RaycastHit hit)
            ? hit.point
            : ray.origin + ray.direction * 200f;

        float scatter = randomRadius;
        Vector2 offset2D = Random.insideUnitCircle * scatter;
        Vector3 offset = new Vector3(offset2D.x, 0f, offset2D.y);

        return baseTarget + offset;
    }
}
