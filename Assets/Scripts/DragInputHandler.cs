using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class DragInputHandler : MonoBehaviour
{
    public event Action<Vector2> DragStarted;
    public event Action<Vector2> DragMoved;
    public event Action<Vector2> DragEnded;

    private Camera cam;
    private InputAction pressAction;
    private InputAction positionAction;
    private bool isDragging;

    private void Awake()
    {
        cam = Camera.main;
        pressAction = new InputAction("Press", InputActionType.Button, "<Pointer>/press");
        positionAction = new InputAction("Position", InputActionType.Value, "<Pointer>/position");
    }

    private void OnEnable()
    {
        pressAction.started += OnPressStarted;
        pressAction.canceled += OnPressCanceled;
        pressAction.Enable();
        positionAction.Enable();
    }

    private void OnDisable()
    {
        pressAction.started -= OnPressStarted;
        pressAction.canceled -= OnPressCanceled;
        pressAction.Disable();
        positionAction.Disable();
    }

    private void OnDestroy()
    {
        pressAction.Dispose();
        positionAction.Dispose();
    }

    private void Update()
    {
        if (isDragging == true)
            DragMoved?.Invoke(ReadWorldPosition());
    }

    private void OnPressStarted(InputAction.CallbackContext context)
    {
        isDragging = true;
        DragStarted?.Invoke(ReadWorldPosition());
    }

    private void OnPressCanceled(InputAction.CallbackContext context)
    {
        isDragging = false;
        DragEnded?.Invoke(ReadWorldPosition());
    }

    private Vector2 ReadWorldPosition()
    {
        Vector2 screenPosition = positionAction.ReadValue<Vector2>();
        return cam.ScreenToWorldPoint(screenPosition);
    }
}