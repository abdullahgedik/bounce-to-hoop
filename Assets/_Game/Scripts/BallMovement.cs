using System;
using System.Collections;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using UnityEngine;

public class BallMovement : MonoBehaviour
{
    public event Action OnThrow;

    [Header("References")]
    [SerializeField] private Rigidbody2D _rb;
    [SerializeField] private LineRenderer _line;

    [Header("Settings")]
    [SerializeField] private float _dragLimit = 3;
    [SerializeField] private float _forceToAdd = 10;
    [SerializeField] private float _throwCooldown = 0.5f;

    private InputAction _throwAction;
    private InputAction _pointAction;
    private Camera _cam;
    private Coroutine _throwCooldownRoutine;
    private bool _isGamePaused = false;
    private bool _isThrowable = true;
    private bool _isDragging;


    private Vector3 MousePosition
    {
        get
        {
            Vector3 pos = _cam.ScreenToWorldPoint(_pointAction.ReadValue<Vector2>());
            pos.z = 0f;
            return pos;
        }
    }

    private void Awake()
    {
        _throwAction = InputSystem.actions.FindAction("Gameplay/Throw");
        _pointAction = InputSystem.actions.FindAction("Gameplay/Point");
        _cam = Camera.main;
    }


    private void Start()
    {
        SetLine();
    }

    private void OnEnable()
    {
        GameManager.OnPauseToggle += GamePauseToggle;
    }

    private void OnDisable()
    {
        GameManager.OnPauseToggle -= GamePauseToggle;
        if (_throwCooldownRoutine != null)
            StopCoroutine(_throwCooldownRoutine);
    }

    private void Update()
    {
        if (_throwAction.WasPressedThisFrame() && !_isDragging && _isThrowable && !_isGamePaused
            && !EventSystem.current.IsPointerOverGameObject())
        {
            DragStart();
        }

        if (_isDragging)
            Drag();

        if (_throwAction.WasReleasedThisFrame() && _isDragging)
        {
            DragEnd();
            _throwCooldownRoutine = StartCoroutine(ThrowTimer());
        }
    }

    private void DragStart()
    {
        _line.enabled = true;
        _isDragging = true;
        _line.SetPosition(0, MousePosition);
    }

    private void Drag()
    {
        Vector3 startPos = _line.GetPosition(0);
        Vector3 currentPos = MousePosition;

        Vector3 distance = currentPos - startPos;

        if (distance.magnitude <= _dragLimit)
        {
            _line.SetPosition(1, currentPos);
        }
        else
        {
            Vector3 limitVector = startPos + (distance.normalized * _dragLimit);
            _line.SetPosition(1, limitVector);
        }
    }

    private void DragEnd()
    {
        _isDragging = false;
        _line.enabled = false;

        Vector3 startPos = _line.GetPosition(0);
        Vector3 currentPos = _line.GetPosition(1);

        Vector3 distance = currentPos - startPos;
        Vector3 finalForce = distance * _forceToAdd;

        OnThrow?.Invoke();
        _rb.AddForce(-finalForce, ForceMode2D.Impulse);
    }

    private IEnumerator ThrowTimer()
    {
        _isThrowable = false;
        yield return new WaitForSeconds(_throwCooldown);
        _isThrowable = true;
    }

    private void GamePauseToggle(bool paused)
    {
        _isGamePaused = paused;

        if (paused && _isDragging)
        {
            _isDragging = false;
            _line.enabled = false;
        }
    }

    private void SetLine()
    {
        _line.positionCount = 2;
        _line.SetPosition(0, Vector2.zero);
        _line.SetPosition(1, Vector2.zero);
        _line.enabled = false;
    }
}