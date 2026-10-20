using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Portal : MonoBehaviour
{
    [Header("Connect Other Portal")]
    [SerializeField] private Transform destination;
    
    private GameObject player;

    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(Vector2.Distance(collision.transform.position, transform.position) > 0.75f)
        {
            player.transform.position = destination.transform.position;
        }
        
    }
}
