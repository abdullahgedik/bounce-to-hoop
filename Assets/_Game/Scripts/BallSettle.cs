using UnityEngine;

public class BallSettle : MonoBehaviour
{
    [Header("Settings")]
    [Tooltip("Speed lost per second while touching the ground.")]
    [SerializeField] private float _rollingResistance = 2f;
    [SerializeField] private float _stopSpeed = 0.2f;
    [Tooltip("Degrees per second.")]
    [SerializeField] private float _stopAngularSpeed = 45f;
    [Tooltip("How long the ball has to stay slow before it is stopped.")]
    [SerializeField] private float _stopDelay = 0.1f;
    [Tooltip("Contacts whose normal points at least this much upward count as ground (0.7 ≈ slopes up to 45°).")]
    [SerializeField, Range(0f, 1f)] private float _minGroundNormalY = 0.7f;
    [Tooltip("The ball still counts as grounded this long after losing contact, so tiny hops don't reset the stop timer.")]
    [SerializeField] private float _groundedGrace = 0.1f;

    private readonly ContactPoint2D[] _contacts = new ContactPoint2D[8];
    private Rigidbody2D _rb;
    private float _slowTime;
    private float _lastGroundedTime = float.NegativeInfinity;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        if (_rb.bodyType != RigidbodyType2D.Dynamic || !_rb.IsAwake() || !IsGrounded())
        {
            _slowTime = 0f;
            return;
        }

        Vector2 velocity = _rb.linearVelocity;
        float speed = velocity.magnitude;
        float slowedSpeed = Mathf.Max(0f, speed - _rollingResistance * Time.fixedDeltaTime);
        if (speed > 0f)
            _rb.linearVelocity = velocity * (slowedSpeed / speed);

        bool isSlow = slowedSpeed < _stopSpeed && Mathf.Abs(_rb.angularVelocity) < _stopAngularSpeed;
        _slowTime = isSlow ? _slowTime + Time.fixedDeltaTime : 0f;

        if (_slowTime >= _stopDelay)
        {
            _rb.linearVelocity = Vector2.zero;
            _rb.angularVelocity = 0f;
            _rb.Sleep();
            _slowTime = 0f;
        }
    }

    private bool IsGrounded()
    {
        int count = _rb.GetContacts(_contacts);
        for (int i = 0; i < count; i++)
        {
            Vector2 normal = (_rb.worldCenterOfMass - _contacts[i].point).normalized;

            if (normal.y >= _minGroundNormalY)
            {
                _lastGroundedTime = Time.fixedTime;
                break;
            }
        }

        return Time.fixedTime - _lastGroundedTime <= _groundedGrace;
    }
}
