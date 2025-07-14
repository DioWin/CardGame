using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class ThrowableItem : MonoBehaviour
{
    [Header("Throw Settings")]
    public float throwForceMultiplier = 20f;
    public float maxThrowForce = 150f;

    [Header("Physics Settings")]
    public float fallDownForce = 200f;

    private Rigidbody rb;
    private bool hasBeenThrown = false;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }
    
    public void SetFallDownForce(float fallDownForce)
    {
        this.fallDownForce = fallDownForce;
    }

    public void ThrowWithVelocity(Vector3 velocity)
    {
        rb.isKinematic = false;
        hasBeenThrown = true;

        Vector3 throwForce = velocity * throwForceMultiplier;
        if (throwForce.magnitude > maxThrowForce)
            throwForce = throwForce.normalized * maxThrowForce;

        rb.AddForce(throwForce, ForceMode.Impulse);
    }

    private void FixedUpdate()
    {
        if (hasBeenThrown)
        {
            rb.AddForce(Vector3.down * fallDownForce, ForceMode.Force);
        }
    }
}
