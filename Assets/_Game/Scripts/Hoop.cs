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

        // Where the ball was one step ago: a fast fall can already be past the rim line on the entering step
        float previousY = ball.position.y - ball.linearVelocity.y * Time.fixedDeltaTime;
        _wasAboveRim = previousY >= RimHeight;

        // Coming up from under the hoop never counts, even if the ball falls back down through it
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

    // Uses the rigidbody, not the transform: with interpolation the transform can lag a step behind
    private void CheckScore(Rigidbody2D ball)
    {
        if (_isScored || _enteredFromBelow)
            return;

        bool isAboveRim = ball.position.y >= RimHeight;

        // Scores on the step the ball's center drops through the rim line between the rims
        if (_wasAboveRim && !isAboveRim && ball.linearVelocity.y < 0f && IsInsideRim(ball.position.x))
        {
            _isScored = true;
            OnHoopScored?.Invoke();
        }

        _wasAboveRim = isAboveRim;
    }

    // Order-independent, so mirrored hoops (scale x -1) work: their "left" rim is on the right
    private bool IsInsideRim(float x)
    {
        float min = Mathf.Min(_rimLeft.position.x, _rimRight.position.x);
        float max = Mathf.Max(_rimLeft.position.x, _rimRight.position.x);
        return x > min && x < max;
    }
}
