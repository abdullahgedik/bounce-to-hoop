using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(CircleCollider2D), typeof(SpriteRenderer))]
[RequireComponent(typeof(TrailRenderer), typeof(BallMovement))]
public class BallSticky : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PhysicsMaterial2D _bouncy;
    [SerializeField] private PhysicsMaterial2D _notBouncy;

    [Header("Settings")]
    [SerializeField] private bool _isSticky;
    [SerializeField] private Color _stickyColor = Color.green;
    [SerializeField] private float _releaseGrace = 0.1f;

    private Rigidbody2D _rb;
    private CircleCollider2D _circleCollider;
    private SpriteRenderer _spriteRenderer;
    private TrailRenderer _trailRenderer;
    private BallMovement _ballMovement;

    private Color _ballColor;
    private Color _trailColor;
    private bool _isStuck;
    private float _canStickTime;
    private Vector2 _lastPosition;
    private Vector2 _lastVelocity;

    public bool IsSticky => _isSticky;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _circleCollider = GetComponent<CircleCollider2D>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _trailRenderer = GetComponent<TrailRenderer>();
        _ballMovement = GetComponent<BallMovement>();

        _ballColor = _spriteRenderer.color;
        _trailColor = _trailRenderer.startColor;
    }

    private void Start()
    {
        ApplyMode();
    }

    private void OnEnable()
    {
        _ballMovement.OnThrow += Release;
    }

    private void OnDisable()
    {
        _ballMovement.OnThrow -= Release;
    }

    // Runs right before each physics step, so on impact this is where the ball came from
    private void FixedUpdate()
    {
        _lastPosition = _rb.position;
        _lastVelocity = _rb.linearVelocity;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        TryStick(collision);
    }

    // Also catches surfaces the ball is still touching once the release grace runs out
    private void OnCollisionStay2D(Collision2D collision)
    {
        TryStick(collision);
    }

    public void SetSticky(bool isSticky)
    {
        if (!isSticky)
            Release();

        _isSticky = isSticky;
        ApplyMode();
    }

    private void ApplyMode()
    {
        _circleCollider.sharedMaterial = _isSticky ? _notBouncy : _bouncy;
        _spriteRenderer.color = _isSticky ? _stickyColor : _ballColor;
        _trailRenderer.startColor = _isSticky ? _stickyColor : _trailColor;
    }

    private void TryStick(Collision2D collision)
    {
        if (!_isSticky || _isStuck || Time.time < _canStickTime || collision.contactCount == 0)
            return;

        _isStuck = true;
        _rb.linearVelocity = Vector2.zero;
        _rb.angularVelocity = 0f;

        // Kinematic bodies ignore gravity and contact forces, so the ball can't slide
        _rb.bodyType = RigidbodyType2D.Kinematic;

        Vector2 impactPosition = GetImpactPosition(collision.GetContact(0));
        _rb.position = impactPosition;
        transform.position = impactPosition;
    }

    // The collision callback comes after the physics step, when the ball has already slid a bit
    // along the surface. Rewind along last step's path to where it first touched the surface.
    private Vector2 GetImpactPosition(ContactPoint2D contact)
    {
        float radius = _circleCollider.radius * Mathf.Max(Mathf.Abs(transform.lossyScale.x), Mathf.Abs(transform.lossyScale.y));
        Vector2 normal = contact.normal;
        Vector2 step = _lastVelocity * Time.fixedDeltaTime;
        float approachSpeed = Vector2.Dot(step, normal);

        // Not moving into the surface (resting or grazing): just sit on it
        if (approachSpeed >= -0.0001f)
            return contact.point + normal * radius;

        float distanceToSurface = Vector2.Dot(_lastPosition - contact.point, normal) - radius;
        float t = Mathf.Clamp01(distanceToSurface / -approachSpeed);

        return _lastPosition + step * t;
    }

    private void Release()
    {
        if (!_isStuck)
            return;

        _isStuck = false;
        _rb.bodyType = RigidbodyType2D.Dynamic;

        // Without this the ball would stick again to the surface it's leaving
        _canStickTime = Time.time + _releaseGrace;
    }
}
