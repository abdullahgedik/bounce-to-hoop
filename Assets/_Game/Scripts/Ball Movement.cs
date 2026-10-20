using System.Collections;
using UnityEngine.InputSystem;
using UnityEngine;

public class BallMovement : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private LineRenderer line;
    [Header("Settings")]
    [SerializeField] private float dragLimit;
    [SerializeField] private float forceToAdd;
    [SerializeField] private float throwCooldown;

    private bool isGamePaused = false;

    private bool isThrowable = true;
    private Camera cam;
    private bool isDragging;
    private InputAction throwAction;
    private InputAction pointAction;


    Vector3 MousePosition
    {
        get
        {
            Vector3 pos = cam.ScreenToWorldPoint(pointAction.ReadValue<Vector2>());
            pos.z = 0f;
            return pos;
        }
    }

    void Awake()
    {
        throwAction = InputSystem.actions.FindAction("Gameplay/Throw");
        pointAction = InputSystem.actions.FindAction("Gameplay/Point");
    }


    void Start()
    {
        cam = Camera.main;
        line.positionCount = 2;
        line.SetPosition(0, Vector2.zero);
        line.SetPosition(1, Vector2.zero);
        line.enabled = false;
    }

    void Update()
    {
        if (throwAction.WasPressedThisFrame() && !isDragging && isThrowable && !isGamePaused)
        {
            DragStart();
        }

        if (isDragging)
            Drag();

        if (throwAction.WasReleasedThisFrame() && isDragging)
        {
            DragEnd();
            StartCoroutine(ThrowTimer());
        }
    }

    void DragStart()
    {
        line.enabled = true;
        isDragging = true;
        line.SetPosition(0, MousePosition);
    }

    void Drag()
    {
        Vector3 startPos = line.GetPosition(0);
        Vector3 currentPos = MousePosition;

        Vector3 distance = currentPos - startPos;

        if (distance.magnitude <= dragLimit)
        {
            line.SetPosition(1, currentPos);
        }
        else
        {
            Vector3 limitVector = startPos + (distance.normalized * dragLimit);
            line.SetPosition(1, limitVector);
        }
    }

    void DragEnd()
    {
        isDragging = false;
        line.enabled = false;

        Vector3 startPos = line.GetPosition(0);
        Vector3 currentPos = line.GetPosition(1);

        Vector3 distance = currentPos - startPos;
        Vector3 finalForce = distance * forceToAdd;

        rb.AddForce(-finalForce, ForceMode2D.Impulse);
    }

    IEnumerator ThrowTimer()
    {
        isThrowable = false;
        yield return new WaitForSeconds(throwCooldown);
        isThrowable = true;
    }

    public void GameIsPaused()
    {
        isGamePaused = !isGamePaused;
    }
}