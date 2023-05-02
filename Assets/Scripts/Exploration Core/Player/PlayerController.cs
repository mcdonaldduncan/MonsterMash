using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Navigator))]
public class PlayerController : MonoBehaviour, IManageable
{
    InputActions InputActions;
    Navigator Navigator;

    RaycastHit Hit;
    Camera MainCamera;

    public bool IsActive { get; set; }

    public delegate void CameraControlDelegate(float increment);
    public event CameraControlDelegate RotateCamera;
    public event CameraControlDelegate ZoomCamera;

    private void OnEnable()
    {
        MainCamera = Camera.main;
        Navigator = GetComponent<Navigator>();

        InputActions = new InputActions();

        SetActive(true); // ToDo remove once home is developed

        InputActions.Player.Select.performed += OnSelect;
        InputActions.Player.Move.started += OnMove;
        InputActions.Player.Move.canceled += OnMove;

        InputActions.Player.Scroll.performed += OnScroll;
    }

    private void Start()
    {
        PrepareTransitions();
    }

    private void OnDestroy()
    {
        InputActions.Player.Select.performed -= OnSelect;
        InputActions.Player.Move.started -= OnMove;
        InputActions.Player.Move.canceled -= OnMove;

        InputActions.Player.Scroll.performed -= OnScroll;
    }

    private void Update()
    {
        if (!IsActive) return;
        if (!Physics.Raycast(MainCamera.ScreenPointToRay(Input.mousePosition), out RaycastHit hit)) return;
        Navigator.SetPath(hit.point);
    }

    public void OnScroll(InputAction.CallbackContext context)
    {
        ZoomCamera?.Invoke(context.ReadValue<float>());
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            RotateCamera?.Invoke(context.ReadValue<Vector2>().x);
        }
        else
        {
            RotateCamera?.Invoke(0);
        }
    }

    public void OnSelect(InputAction.CallbackContext context)
    {
        if (!Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out Hit)) return;

        if (Hit.collider.gameObject.CompareTag("Enemy"))
        {
            Navigator.MoveToLocation(Hit.collider.gameObject.transform, true);
            Navigator.StopMove += BattleTransition;
        }
        else
        {
            Navigator.StopMove -= BattleTransition;
            Navigator.MoveToLocation(Hit.point);
        }
    }

    private void BattleTransition()
    {
        TransitionManager.Instance.Transition(GameState.BATTLE);
    }

    public void SetActive(bool active)
    {
        if (IsActive == active) return;

        IsActive = active;

        if (IsActive) Initialize();
        else Sleep();
    }

    public void Initialize()
    {
        Cursor.visible = false;

        InputActions.Player.Enable();
    }

    public void Sleep()
    {
        Cursor.visible = true;

        InputActions.Player.Disable();

        Navigator.Sleep();
    }

    public void PrepareTransitions()
    {
        TransitionManager.Instance.SubscribeToTransition(GameState.EXPLORATION, Enable);
        TransitionManager.Instance.SubscribeToTransition(GameState.BATTLE, Disable);
        TransitionManager.Instance.SubscribeToTransition(GameState.HOME, Disable);
    }

    public void DisableTransitions()
    {
        TransitionManager.Instance.UnsubscribeFromTransition(GameState.EXPLORATION, Enable);
        TransitionManager.Instance.UnsubscribeFromTransition(GameState.BATTLE, Disable);
        TransitionManager.Instance.UnsubscribeFromTransition(GameState.HOME, Disable);
    }

    public void Enable()
    {
        SetActive(true);
    }

    public void Disable()
    {
        SetActive(false);
    }
}
