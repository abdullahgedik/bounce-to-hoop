using System.Collections;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float _speed;
    [SerializeField] private float _patrolDuration;
    [SerializeField] private Rigidbody2D _rb;

    private float _direction = 1;
    private float _patrolStartDirection;
    private Vector2 _patrolLocation;
    private Coroutine _patrolRoutine;

    private void Start()
    {
        _direction = Mathf.Sign(transform.localScale.x);
        _patrolLocation = transform.position;
        _patrolStartDirection = _direction;

        _patrolRoutine = StartCoroutine(ChangeDirection());
    }

    private void OnDisable()
    {
        if (_patrolRoutine != null)
            StopCoroutine(_patrolRoutine);
    }

    private void FixedUpdate()
    {
        _rb.linearVelocity = new Vector2(_direction * _speed, _rb.linearVelocity.y);
    }

    private IEnumerator ChangeDirection()
    {
        while (true)
        {
            yield return new WaitForSeconds(_patrolDuration);
            MirrorEnemy();
        }
    }

    private void MirrorEnemy()
    {
        transform.localScale = new Vector2(transform.localScale.x * -1, transform.localScale.y);
        _direction *= -1;
    }

    private void OnDrawGizmos()
    {
        Vector2 origin = Application.isPlaying ? _patrolLocation : (Vector2)transform.position;
        float dir = Application.isPlaying ? _patrolStartDirection : Mathf.Sign(transform.localScale.x);
        Gizmos.DrawLine(origin, origin + Vector2.right * (_speed * _patrolDuration * dir));
    }
}
