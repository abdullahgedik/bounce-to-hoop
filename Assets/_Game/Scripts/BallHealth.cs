using System;
using UnityEngine;

public class BallHealth : MonoBehaviour
{
    public static event Action OnBallDeath;

    [Header("Settings")]
    [SerializeField] private int _maxHealth;

    private int _health;
    private bool _canTakeDamage;

    void Start()
    {
        _canTakeDamage = true;
        _health = _maxHealth;
    }

    private void OnEnable()
    {
        Hoop.OnHoopScored += EnableDamageImmunity;
    }

    private void OnDisable()
    {
        Hoop.OnHoopScored -= EnableDamageImmunity;
    }

    public void TakeDamage(int amount)
    {
        if (!_canTakeDamage)
            return;

        _health -= amount;

        if (_health <= 0)
        {
            OnBallDeath?.Invoke();
            gameObject.SetActive(false);
        }
    }

    private void EnableDamageImmunity()
    {
        _canTakeDamage = false;
    }
}
