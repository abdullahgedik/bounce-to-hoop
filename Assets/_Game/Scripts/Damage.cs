using UnityEngine;

public class Damage : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private int _damage;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.TryGetComponent(out BallHealth ballHealth))
        {
            ballHealth.TakeDamage(_damage);
        }
    }
}
