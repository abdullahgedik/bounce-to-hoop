using UnityEngine;

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

    private void FixedUpdate()
    {
        _lastVelocity = _rb.linearVelocity;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (_rb.bodyType != RigidbodyType2D.Dynamic || collision.contactCount == 0)
            return;

        Vector2 normal = (_rb.worldCenterOfMass - collision.GetContact(0).point).normalized;
        float impactSpeed = -Vector2.Dot(_lastVelocity, normal);

        if (impactSpeed <= 0f)
            return;

        PhysicsMaterial2D material = _circleCollider.sharedMaterial;
        float bounciness = material != null ? material.bounciness : 0f;
        float fade = Mathf.InverseLerp(_minBounceSpeed, _fullBounceSpeed, impactSpeed);
        float bounceSpeed = impactSpeed * bounciness * fade;

        Vector2 velocity = _rb.linearVelocity;
        float normalSpeed = Vector2.Dot(velocity, normal);
        _rb.linearVelocity = velocity + normal * (bounceSpeed - normalSpeed);
    }
}
