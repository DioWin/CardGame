using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(ThrowableItem))]
public class BombSpellInstance : RuntimeSpellBase
{
    private ThrowableItem throwable;
    private BombSpell bombConfig;
    private Rigidbody rigidbody;
    private bool isFollowingCursor = false;
    private bool isThrown = false;

    private void Awake()
    {
        throwable = GetComponent<ThrowableItem>();
        throwable.enabled = false;

        rigidbody = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        if (isFollowingCursor && !isThrown)
        {
            transform.position = GetStartPosition();
        }

        UpdateSpell();
    }

    public override void Init(SpellBase config, GameObject caster, CardController cardController, Vector3 throwVelocity)
    {
        base.Init(config, caster, cardController, throwVelocity);
        bombConfig = config as BombSpell;

        transform.position = GetStartPosition();
        throwable.enabled = false;
        isThrown = false;
        isFollowingCursor = false;

        // Subscribe to CardView events

        cardController.OnFollowStart += EnableFollow;
        cardController.OnFollowStop += DisableFollow;
        cardController.OnThrowConfirmed += ThrowConfirmed;
        gameObject.SetActive(false);
    }

    private void ThrowConfirmed()
    {
        if (!isThrown)
        {
            gameObject.SetActive(true);

            Debug.Log("OnThrowConfirmed");

            isThrown = true;
            isFollowingCursor = false;

            throwable.enabled = true;
            Debug.Log(cardController.GetThrowVelocity());

            throwable.ThrowWithVelocity(cardController.GetThrowVelocity());
            CurrentState = SpellState.Active;
        }
    }

    private void EnableFollow()
    {
        rigidbody.isKinematic = true;

        gameObject.SetActive(true);

        Debug.Log("EnableFollow");
        isFollowingCursor = true;
        throwable.enabled = false;
    }

    private void DisableFollow()
    {
        rigidbody.isKinematic = false;

        gameObject.SetActive(false);

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
}
