using System;
using UnityEngine;

public class Hoop : MonoBehaviour
{
    public static event Action OnHoopScored;

    private bool _isScored = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (_isScored || !collision.CompareTag("Player"))
            return;

        _isScored = true;
        OnHoopScored?.Invoke();
    }
}
