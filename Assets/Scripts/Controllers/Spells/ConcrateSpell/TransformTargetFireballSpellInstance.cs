using UnityEngine;

public class TransformTargetFireballSpellInstance : RuntimeSpellBase
{
    private FireballSpell fireballConfig;
    private ThrowableItem throwable;
    private RaycastFlightController raycastFlight;

    public float randomRadius = 1f;

    public float minMagnitudaToThrow;

    private void Awake()
    {
        throwable = GetComponent<ThrowableItem>();
        raycastFlight = GetComponent<RaycastFlightController>();
    }

    public override void Init(SpellBase config, GameObject caster, CardController cardController, Vector3 throwVelocity)
    {
        base.Init(config, caster, cardController, throwVelocity);
        fireballConfig = config as FireballSpell;

        if (throwVelocity.magnitude > minMagnitudaToThrow)
        {
            // Use physics-based throw
            transform.position = GetStartPosition();
            throwable.ThrowWithVelocity(throwVelocity);

            raycastFlight.enabled = false;
        }
        else
        {
            // Use raycast controller
            Vector3 start = GetStartPosition();
            Vector3 target = raycastFlight.GetRaycastHitPoint();

            raycastFlight.Init(start, target, fireballConfig.speed, fireballConfig.arcHeight);

            throwable.enabled = false;
        }

        CurrentState = SpellState.Active;
    }

    private void Update()
    {
        UpdateSpell();
    }

    protected override void OnPrepare() { }

    protected override void OnActive()
    {
        if (throwVelocity.magnitude <= 0.1f && raycastFlight.HasArrived)
        {
            Explode();
            CurrentState = SpellState.Finished;
        }
    }

    protected override void OnFinished()
    {
        Destroy(gameObject);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (throwVelocity.magnitude > 0.1f && CurrentState == SpellState.Active)
        {
            Explode();
            CurrentState = SpellState.Finished;
        }
    }

    private void Explode()
    {
        float damage = fireballConfig.damage;
        float radius = fireballConfig.radius;

        Collider[] hits = Physics.OverlapSphere(transform.position, radius);
        foreach (var hit in hits)
        {
            if (hit.TryGetComponent<IDamageable>(out var dmg))
                dmg.TakeDamage(damage);
        }

        Debug.Log($"Boom! Damage: {damage}, Radius: {radius}");
    }

    private Vector3 GetStartPosition()
    {
        float baseTrackingDistance = 20f;
        float trackingMultiplier = 1f;
        Camera cam = Camera.main;

        float yRatio = Mathf.Clamp01(Input.mousePosition.y / Screen.height);
        float dynamicDistance = baseTrackingDistance * Mathf.Lerp(1f, trackingMultiplier, yRatio);

        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        Plane plane = new Plane(-cam.transform.forward, cam.transform.position + cam.transform.forward * dynamicDistance);

        if (plane.Raycast(ray, out float enter))
            return ray.GetPoint(enter);

        return Vector3.zero;
    }

    private void OnDrawGizmos()
    {
        if (fireballConfig == null) return;

        Gizmos.color = new Color(1f, 0.3f, 0.1f, 0.4f);
        Gizmos.DrawSphere(transform.position, fireballConfig.radius);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, fireballConfig.radius);
    }
}
