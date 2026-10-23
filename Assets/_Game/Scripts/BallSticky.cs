using UnityEngine;

public class BallSticky : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PhysicsMaterial2D _bouncy;
    [SerializeField] private PhysicsMaterial2D _notBouncy;
    [SerializeField] private SpriteRenderer _spriteRenderer;
    [SerializeField] private Sprite _normalBallSprite;
    [SerializeField] private Sprite _stickyBallSprite;


    [Header("Settings")]
    [SerializeField] private bool _isSticky;
    [SerializeField] private Color _stickyColor = Color.green;
    [SerializeField] private float _releaseGrace = 0.1f;

    private Rigidbody2D _rb;
    private CircleCollider2D _circleCollider;
    private TrailRenderer _trailRenderer;
    private BallMovement _ballMovement;

    private Color _normalTrailColor;
    private bool _isStuck;
    private float _canStickTime;
    private Vector2 _lastPosition;
    private Vector2 _lastVelocity;

    public bool IsSticky => _isSticky;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _circleCollider = GetComponent<CircleCollider2D>();
        _trailRenderer = GetComponent<TrailRenderer>();
        _ballMovement = GetComponent<BallMovement>();

        _normalTrailColor = _trailRenderer.startColor;
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

    private void FixedUpdate()
    {
        _lastPosition = _rb.position;
        _lastVelocity = _rb.linearVelocity;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        TryStick(collision);
    }

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
        _trailRenderer.startColor = _isSticky ? _stickyColor : _normalTrailColor;
        _spriteRenderer.sprite = _isSticky ? _stickyBallSprite : _normalBallSprite;
    }

    private void TryStick(Collision2D collision)
    {
        if (!_isSticky || _isStuck || Time.time < _canStickTime || collision.contactCount == 0)
            return;

        _isStuck = true;
        _rb.linearVelocity = Vector2.zero;
        _rb.angularVelocity = 0f;

        _rb.bodyType = RigidbodyType2D.Kinematic;

        Vector2 impactPosition = GetImpactPosition(collision.GetContact(0));
        _rb.position = impactPosition;
        transform.position = impactPosition;
    }

    private Vector2 GetImpactPosition(ContactPoint2D contact)
    {
        float radius = _circleCollider.radius * Mathf.Max(Mathf.Abs(transform.lossyScale.x), Mathf.Abs(transform.lossyScale.y));
        Vector2 normal = contact.normal;
        Vector2 step = _lastVelocity * Time.fixedDeltaTime;
        float approachSpeed = Vector2.Dot(step, normal);

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

        _canStickTime = Time.time + _releaseGrace;
    }
}
