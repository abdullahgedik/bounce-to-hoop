using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameManager gameManager;
    [Header("Settings")]
    [SerializeField] private int maxHealth;

    private int health;

    void Start()
    {
        health = maxHealth;
    }

    public void TakeDamage(int amount)
    {
        health -= amount;
        if (health <= 0)
        {
            gameManager.PlayerIsDead();
            Destroy(gameObject);
        }
    }
}
