using UnityEngine;

// Unity's bounce is all-or-nothing: an impact faster than Physics 2D's Bounce Threshold bounces with
// the material's full bounciness, anything slower doesn't bounce at all. This fades the bounce in
// instead: slow impacts land dead, fast ones get the full bounciness, and impacts in between get a
// share of it. That removes the chain of tiny bounces before the ball settles.
public class BallBounce : MonoBehaviour
{
    [Header("Settings")]
    [Tooltip("Impacts slower than this don't bounce.")]
    [SerializeField] private float _minBounceSpeed = 3f;
    [Tooltip("Impacts faster than this bounce with the material's full bounciness.")]
    [SerializeField] private float _fullBounceSpeed = 8f;

    private Rigidbody2D _rb;
    private CircleCollider2D _circleCollider;
    private Vector2 _lastVelocity;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _circleCollider = GetComponent<CircleCollider2D>();
    }

    // Runs right before each physics step, so on a collision this is the velocity the ball arrived with
    private void FixedUpdate()
    {
        _lastVelocity = _rb.linearVelocity;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // A stuck sticky ball is kinematic; it shouldn't be pushed off the surface
        if (_rb.bodyType != RigidbodyType2D.Dynamic || collision.contactCount == 0)
            return;

        // For a circle, the direction from the contact point to the center is the surface normal
        Vector2 normal = (_rb.worldCenterOfMass - collision.GetContact(0).point).normalized;
        float impactSpeed = -Vector2.Dot(_lastVelocity, normal);

        // Not an impact: e.g. a sticky ball thrown off the surface it was stuck to starts a new
        // contact while already moving away from it, and cancelling that would kill the throw
        if (impactSpeed <= 0f)
            return;

        // The sticky ball swaps to a material with no bounciness, so it never bounces here either
        PhysicsMaterial2D material = _circleCollider.sharedMaterial;
        float bounciness = material != null ? material.bounciness : 0f;
        float fade = Mathf.InverseLerp(_minBounceSpeed, _fullBounceSpeed, impactSpeed);
        float bounceSpeed = impactSpeed * bounciness * fade;

        // Replace whatever bounce the physics engine applied along the normal with the faded one
        Vector2 velocity = _rb.linearVelocity;
        float normalSpeed = Vector2.Dot(velocity, normal);
        _rb.linearVelocity = velocity + normal * (bounceSpeed - normalSpeed);
    }
}
