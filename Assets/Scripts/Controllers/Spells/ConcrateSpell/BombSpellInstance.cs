using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;

[RequireComponent(typeof(ThrowableItem))]
public class BombSpellInstance : RuntimeSpellBase
{
    [SerializeField] private float fallowSpeedMultiplier = 25;

    private RenderQueueFollower renderQueue;
    private ThrowableItem throwable;
    private BombSpell bombConfig;
    private Rigidbody rb;

    private bool isFollowingCursor = false;
    private bool isThrown = false;

    private void Awake()
    {
        throwable = GetComponent<ThrowableItem>();
        throwable.enabled = false;

        rb = GetComponent<Rigidbody>();
        renderQueue = GetComponent<RenderQueueFollower>();
        rb.isKinematic = true;
    }

    public override void Init(SpellBase config, GameObject caster, CardController cardController, Vector3 throwVelocity)
    {
        base.Init(config, caster, cardController, throwVelocity);
        bombConfig = config as BombSpell;

        transform.SetParent(cardController.GetVisualTransform());

        Vector3 screenPos = Camera.main.WorldToScreenPoint(cardController.GetVisualTransform().position);
        var targetPos = GetWorldPositionFromScreen(screenPos);

        transform.position = targetPos;
        throwable.enabled = false;
        isThrown = false;
        isFollowingCursor = false;

        // Subscribe to CardView events

        cardController.OnRelease += OnRelease;
        cardController.OnEndDragging += OnEndDragging;
        cardController.OnFollowStart += EnableFollow;
        cardController.OnFollowStop += DisableFollow;
        cardController.OnThrowConfirmed += ThrowConfirmed;
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

    private void ThrowConfirmed()
    {
        if (!isThrown)
        {
            Debug.Log("OnThrowConfirmed");

            rb.isKinematic = false;

            transform.SetParent(null);

            isThrown = true;
            isFollowingCursor = false;

            throwable.enabled = true;

            renderQueue.Detach();
            throwable.ThrowWithVelocity(cardController.GetThrowVelocity());
            CurrentState = SpellState.Active;
        }
    }

    private void OnEndDragging()
    {
        renderQueue.SetFallowStatus(false);
        Debug.Log("OnEndDragging");
    }

    private void OnRelease()
    {
        renderQueue.SetFallowStatus(true);
        Debug.Log("OnRelease");
    }

    private void EnableFollow()
    {
        rb.isKinematic = true;

        Debug.Log("EnableFollow");
        isFollowingCursor = true;
        throwable.enabled = false;
    }

    private void DisableFollow()
    {
        Debug.Log("DisableFollow");

        isFollowingCursor = false;
        throwable.enabled = false;
    }

    private Vector3 GetStartPosition()
    {
        float baseTrackingDistance = 20f;
        float trackingMultiplier = 1f;
        Camera mainCamera = Camera.main;

        float mouseY01 = Mathf.Clamp01(Input.mousePosition.y / Screen.height);
        float dynamicDistance = baseTrackingDistance * Mathf.Lerp(1f, trackingMultiplier, mouseY01);

        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        Vector3 planeOrigin = mainCamera.transform.position + mainCamera.transform.forward * dynamicDistance;
        Plane movementPlane = new Plane(-mainCamera.transform.forward, planeOrigin);

        if (movementPlane.Raycast(ray, out float enter))
        {
            return ray.GetPoint(enter);
        }

        return Vector3.zero;
    }

    private Vector3 GetWorldPositionFromScreen(Vector3 screenPosition)
    {
        float baseTrackingDistance = 20f;
        float trackingMultiplier = 1f;
        Camera mainCamera = Camera.main;

        float mouseY01 = Mathf.Clamp01(screenPosition.y / Screen.height);
        float dynamicDistance = baseTrackingDistance * Mathf.Lerp(1f, trackingMultiplier, mouseY01);

        Ray ray = mainCamera.ScreenPointToRay(screenPosition);
        Vector3 planeOrigin = mainCamera.transform.position + mainCamera.transform.forward * dynamicDistance;
        Plane movementPlane = new Plane(-mainCamera.transform.forward, planeOrigin);

        if (movementPlane.Raycast(ray, out float enter))
        {
            return ray.GetPoint(enter);
        }

        return Vector3.zero;
    }

    protected override void OnPrepare()
    {
    }

    protected override void OnActive()
    {
        // Optional: logic while flying
    }

    protected override void OnFinished()
    {
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (CurrentState == SpellState.Active)
        {
            StartCoroutine(DelayBeforeExplode());
            CurrentState = SpellState.Finished;
        }
    }

    private IEnumerator DelayBeforeExplode()
    {
        yield return new WaitForSeconds(bombConfig.delayBeforeExplosive);
        Explode();
    }

    private void Explode()
    {
        float damage = bombConfig.damage;
        float radius = bombConfig.radius;

        Collider[] hitColliders = Physics.OverlapSphere(transform.position, radius);
        foreach (var hit in hitColliders)
        {
            if (hit.TryGetComponent<IDamageable>(out var damageable))
            {
                damageable.TakeDamage(damage);
            }
        }

        DebugCreateExplosionEffect(transform.position, radius);

        Debug.Log($"Boom! Damage: {damage}, Radius: {radius}");
        Destroy(gameObject);
    }

    private void OnDrawGizmos()
    {
        if (bombConfig == null) return;

        Gizmos.color = new Color(1f, 0.3f, 0.1f, 0.4f);
        Gizmos.DrawSphere(transform.position, bombConfig.radius);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, bombConfig.radius);
    }

    public static void DebugCreateExplosionEffect(Vector3 position, float radius, float duration = 0.5f)
    {
        GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        sphere.transform.SetParent(null);
        sphere.transform.position = position;
        sphere.transform.localScale = Vector3.one * radius * 2f;

        sphere.GetComponent<Collider>().isTrigger = true;

        Material mat = new Material(Shader.Find("Standard"));
        Color color = Color.cyan;
        color.a = 0.3f;
        mat.color = color;
        mat.SetFloat("_Mode", 3);
        mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
        mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
        mat.SetInt("_ZWrite", 0);
        mat.DisableKeyword("_ALPHATEST_ON");
        mat.EnableKeyword("_ALPHABLEND_ON");
        mat.DisableKeyword("_ALPHAPREMULTIPLY_ON");
        mat.renderQueue = 3000;

        sphere.GetComponent<Renderer>().material = mat;

        Destroy(sphere, duration);
    }
}
