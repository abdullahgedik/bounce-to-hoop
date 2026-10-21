using UnityEngine;

public class Portal : MonoBehaviour
{
    [Header("Connect Other Portal")]
    [SerializeField] private Transform _destination;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player"))
            return;

        if (Vector2.Distance(collision.transform.position, transform.position) > 0.75f)
            collision.transform.position = _destination.position;
    }
}
