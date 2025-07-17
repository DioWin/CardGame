using UnityEngine;

public class TransformTargetFireballSpellInstance : RuntimeSpellBase
{
    private RenderQueueFollower renderQueue;
    private FireballSpell fireballConfig;
    private ThrowableItem throwable;
    private RaycastFlightController raycastFlight;
    private Rigidbody rb;

    public float randomRadius = 1f;
    public float minMagnitudaToThrow = 1f;
    [SerializeField] private float fallowSpeedMultiplier = 25f;

    private bool isFollowingCursor = false;
    private bool isThrown = false;

    private void Awake()
    {
        throwable = GetComponent<ThrowableItem>();
        raycastFlight = GetComponent<RaycastFlightController>();
        renderQueue = GetComponent<RenderQueueFollower>();
        rb = GetComponent<Rigidbody>();
        throwable.enabled = false;
    }

    public override void Init(SpellBase config, GameObject caster, CardController cardController, Vector3 throwVelocity)
    {
        base.Init(config, caster, cardController, throwVelocity);
        fireballConfig = config as FireballSpell;

        transform.SetParent(cardController.GetVisualTransform());

        Vector3 screenPos = Camera.main.WorldToScreenPoint(cardController.GetVisualTransform().position);
        transform.position = GetWorldPositionFromScreen(screenPos);

        isThrown = false;
        isFollowingCursor = false;
        throwable.enabled = false;
        raycastFlight.enabled = false;

        cardController.OnReleaseEvent += OnRelease;
        cardController.OnEndDraggingEvent += OnEndDragging;
        cardController.OnFollowStartEvent += EnableFollow;
        cardController.OnFollowStopEvent += DisableFollow;
        cardController.OnThrowConfirmedEvent += ThrowConfirmed;

        CurrentState = SpellState.Preparing;
    }

    private void Update()
    {
        if (!isThrown)
            HandleFallowBehavior();

        UpdateSpell();
    }

    private void HandleFallowBehavior()
    {
        Vector3 targetPos = Vector3.zero;

        if (isFollowingCursor)
        {
            targetPos = GetWorldPositionFromScreen(Input.mousePosition);

            transform.position = Vector3.Lerp(transform.position, targetPos, Time.deltaTime * fallowSpeedMultiplier);
        }
        else if (cardController != null)
        {
            Vector3 screenPos = Camera.main.WorldToScreenPoint(cardController.GetVisualTransform().position);
            targetPos = GetWorldPositionFromScreen(screenPos);

            transform.position = targetPos;
        }
    }

    private void OnEndDragging()
    {
        renderQueue.SetFallowStatus(false);
    }

    private void OnRelease()
    {
        renderQueue.SetFallowStatus(true);
    }

    private void EnableFollow()
    {
        isFollowingCursor = true;
        throwable.enabled = false;
    }

    private void DisableFollow()
    {
        isFollowingCursor = false;
        throwable.enabled = false;
    }

    private void ThrowConfirmed()
    {
        if (isThrown) return;
   
        isThrown = true;
        isFollowingCursor = false;

        throwVelocity = cardController.GetThrowVelocity();

        renderQueue.Detach();
        transform.SetParent(null);

        if (throwVelocity.magnitude > minMagnitudaToThrow)
        {
            rb.isKinematic = false;

            throwable.enabled = true;
            throwable.ThrowWithVelocity(throwVelocity);
            raycastFlight.enabled = false;
        }
        else
        {
            throwable.enabled = false;
            raycastFlight.enabled = true;

            rb.isKinematic = false;

            Vector3 start = transform.position;
            Vector3 target = raycastFlight.GetRaycastHitPoint();

            raycastFlight.Init(start, target, fireballConfig.speed, fireballConfig.arcHeight);
        }

        CurrentState = SpellState.Active;
    }

    protected override void OnPrepare() { }

    protected override void OnActive()
    {
        if (throwVelocity.magnitude <= minMagnitudaToThrow && raycastFlight.HasArrived)
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
        if (throwVelocity.magnitude > minMagnitudaToThrow && CurrentState == SpellState.Active)
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

    private Vector3 GetWorldPositionFromScreen(Vector3 screenPosition)
    {
        float baseTrackingDistance = 20f;
        float trackingMultiplier = 1f;
        Camera cam = Camera.main;

        float yRatio = Mathf.Clamp01(screenPosition.y / Screen.height);
        float dynamicDistance = baseTrackingDistance * Mathf.Lerp(1f, trackingMultiplier, yRatio);

        Ray ray = cam.ScreenPointToRay(screenPosition);
        Vector3 planeOrigin = cam.transform.position + cam.transform.forward * dynamicDistance;
        Plane plane = new Plane(-cam.transform.forward, planeOrigin);

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
