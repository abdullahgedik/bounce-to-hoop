using System;
using System.Collections;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using UnityEngine.Rendering;
using UnityEngine;

public class BallMovement : MonoBehaviour
{
    public event Action OnThrow;
    public event Action<Vector2> OnAim;
    public event Action OnAimEnd;

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

    // The drag starts where the player pressed on the screen, not at a point in the world,
    // so the line stays under the finger while the camera follows the ball
    private Vector2 _dragStartScreen;

    private Vector2 PointerScreenPosition => _pointAction.ReadValue<Vector2>();

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
        RenderPipelineManager.beginCameraRendering += UpdateLine;
    }

    private void OnDisable()
    {
        GameManager.OnPauseToggle -= GamePauseToggle;
        RenderPipelineManager.beginCameraRendering -= UpdateLine;
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
        _isDragging = true;
        _dragStartScreen = PointerScreenPosition;
    }

    private void Drag()
    {
        OnAim?.Invoke(GetThrowImpulse());
    }

    private void DragEnd()
    {
        Vector2 impulse = GetThrowImpulse();

        _isDragging = false;
        _line.enabled = false;

        OnAimEnd?.Invoke();
        OnThrow?.Invoke();
        _rb.AddForce(impulse, ForceMode2D.Impulse);
    }

    private Vector2 GetDragVector()
    {
        Vector2 drag = ScreenToWorld(PointerScreenPosition) - ScreenToWorld(_dragStartScreen);
        return Vector2.ClampMagnitude(drag, _dragLimit);
    }

    private Vector2 GetThrowImpulse()
    {
        return -GetDragVector() * _forceToAdd;
    }

    private Vector3 ScreenToWorld(Vector2 screenPosition)
    {
        Vector3 position = _cam.ScreenToWorldPoint(screenPosition);
        position.z = 0f;
        return position;
    }

    private void UpdateLine(ScriptableRenderContext context, Camera camera)
    {
        if (!_isDragging || camera != _cam)
            return;

        Vector3 start = ScreenToWorld(_dragStartScreen);
        _line.SetPosition(0, start);
        _line.SetPosition(1, start + (Vector3)GetDragVector());
        _line.enabled = true;
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
            OnAimEnd?.Invoke();
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