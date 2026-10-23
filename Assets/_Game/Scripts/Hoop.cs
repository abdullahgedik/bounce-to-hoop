using System;
using UnityEngine;

public class Hoop : MonoBehaviour
{
    public static event Action OnHoopScored;

    [Header("References")]
    [SerializeField] private Transform _rimLeft;
    [SerializeField] private Transform _rimRight;

    private bool _isScored = false;
    private bool _enteredFromBelow;
    private bool _wasAboveRim;

    private float RimHeight => transform.position.y;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player"))
            return;

        Rigidbody2D ball = collision.attachedRigidbody;

        float previousY = ball.position.y - ball.linearVelocity.y * Time.fixedDeltaTime;
        _wasAboveRim = previousY >= RimHeight;

        _enteredFromBelow = !_wasAboveRim && ball.linearVelocity.y > 0f;

        CheckScore(ball);
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player"))
            return;

        CheckScore(collision.attachedRigidbody);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
            _enteredFromBelow = false;
    }

    private void CheckScore(Rigidbody2D ball)
    {
        if (_isScored || _enteredFromBelow)
            return;

        bool isAboveRim = ball.position.y >= RimHeight;

        if (_wasAboveRim && !isAboveRim && ball.linearVelocity.y < 0f && IsInsideRim(ball.position.x))
        {
            _isScored = true;
            OnHoopScored?.Invoke();
        }

        _wasAboveRim = isAboveRim;
    }

    private bool IsInsideRim(float x)
    {
        float min = Mathf.Min(_rimLeft.position.x, _rimRight.position.x);
        float max = Mathf.Max(_rimLeft.position.x, _rimRight.position.x);
        return x > min && x < max;
    }
}
