using System.Collections.Generic;
using UnityEngine;

public class TrajectoryPreview : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Sprite _dotSprite;

    [Header("Settings")]
    [SerializeField] private int _dotCount = 10;
    [Tooltip("Physics steps between two dots; higher spreads the dots further apart.")]
    [SerializeField] private int _stepsPerDot = 3;
    [SerializeField] private Color _dotColor = Color.white;
    [Tooltip("Alpha of the last dot; the dots fade out towards the end.")]
    [SerializeField, Range(0f, 1f)] private float _endAlpha = 0.15f;
    [SerializeField] private int _sortingOrder = 10;

    private readonly List<SpriteRenderer> _dots = new();
    private readonly List<RaycastHit2D> _hits = new();
    private Rigidbody2D _rb;
    private CircleCollider2D _circleCollider;
    private BallMovement _ballMovement;
    private Transform _dotRoot;
    private ContactFilter2D _contactFilter;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _circleCollider = GetComponent<CircleCollider2D>();
        _ballMovement = GetComponent<BallMovement>();

        _contactFilter = new ContactFilter2D { useTriggers = false };
        _contactFilter.SetLayerMask(Physics2D.GetLayerCollisionMask(gameObject.layer));

        CreateDots();
    }

    private void OnEnable()
    {
        _ballMovement.OnAim += Show;
        _ballMovement.OnAimEnd += Hide;
    }

    private void OnDisable()
    {
        _ballMovement.OnAim -= Show;
        _ballMovement.OnAimEnd -= Hide;
        Hide();
    }

    private void OnDestroy()
    {
        if (_dotRoot != null)
            Destroy(_dotRoot.gameObject);
    }

    private void Show(Vector2 impulse)
    {
        float deltaTime = Time.fixedDeltaTime;
        Vector2 gravity = Physics2D.gravity * _rb.gravityScale;
        float damping = 1f / (1f + deltaTime * _rb.linearDamping);

        Vector2 startVelocity = _rb.bodyType == RigidbodyType2D.Dynamic ? _rb.linearVelocity : Vector2.zero;
        Vector2 velocity = startVelocity + impulse / _rb.mass;
        Vector2 position = _rb.position;

        float radius = _circleCollider.radius * Mathf.Max(Mathf.Abs(transform.lossyScale.x), Mathf.Abs(transform.lossyScale.y)) * 0.9f;
        bool isBlocked = false;

        foreach (SpriteRenderer dot in _dots)
        {
            for (int step = 0; step < _stepsPerDot && !isBlocked; step++)
            {
                velocity += gravity * deltaTime;
                velocity *= damping;

                Vector2 move = velocity * deltaTime;
                isBlocked = HitsSurface(position, move, radius);

                if (!isBlocked)
                    position += move;
            }

            dot.enabled = !isBlocked;
            dot.transform.position = position;
        }
    }

    private void Hide()
    {
        if (_dotRoot == null)
            return;

        foreach (SpriteRenderer dot in _dots)
            dot.enabled = false;
    }

    private bool HitsSurface(Vector2 from, Vector2 move, float radius)
    {
        float distance = move.magnitude;
        if (distance <= 0f)
            return false;

        Vector2 direction = move / distance;
        int count = Physics2D.CircleCast(from, radius, direction, _contactFilter, _hits, distance);

        for (int i = 0; i < count; i++)
        {
            if (_hits[i].collider == _circleCollider)
                continue;

            if (Vector2.Dot(direction, _hits[i].normal) >= 0f)
                continue;

            return true;
        }

        return false;
    }

    private void CreateDots()
    {
        _dotRoot = new GameObject($"{name} Trajectory").transform;

        for (int i = 0; i < _dotCount; i++)
        {
            var dot = new GameObject($"Dot {i}").AddComponent<SpriteRenderer>();
            dot.transform.SetParent(_dotRoot, false);
            dot.sprite = _dotSprite;
            dot.sortingOrder = _sortingOrder;

            float progress = _dotCount > 1 ? (float)i / (_dotCount - 1) : 0f;
            Color color = _dotColor;
            color.a = Mathf.Lerp(_dotColor.a, _endAlpha, progress);
            dot.color = color;

            dot.enabled = false;
            _dots.Add(dot);
        }
    }
}
