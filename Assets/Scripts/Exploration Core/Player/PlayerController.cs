using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Navigator))]
[RequireComponent(typeof(BattleMonster))]
public class PlayerController : MonoBehaviour, IManageable
{
    BattleMonster m_BattleMonster;
    InputActions m_InputActions;
    Navigator m_Navigator;

    RaycastHit m_Hit;
    Camera m_Camera;

    public bool IsActive { get; set; }

    public delegate void CameraControlDelegate(float increment);
    public event CameraControlDelegate RotateCamera;
    public event CameraControlDelegate ZoomCamera;

    private void OnEnable()
    {
        m_Camera = Camera.main;
        m_Navigator = GetComponent<Navigator>();
        m_BattleMonster = GetComponent<BattleMonster>();

        m_InputActions = new InputActions();

        SetActive(true); // ToDo remove once home is developed

        m_InputActions.Player.Select.performed += OnSelect;
        m_InputActions.Player.Move.started += OnMove;
        m_InputActions.Player.Move.canceled += OnMove;

        m_InputActions.Player.Scroll.performed += OnScroll;
    }

    private void Start()
    {
        PrepareTransitions();
    }

    private void OnDestroy()
    {
        m_InputActions.Player.Select.performed -= OnSelect;
        m_InputActions.Player.Move.started -= OnMove;
        m_InputActions.Player.Move.canceled -= OnMove;

        m_InputActions.Player.Scroll.performed -= OnScroll;
    }

    private void Update()
    {
        if (!IsActive) return;
        if (!Physics.Raycast(m_Camera.ScreenPointToRay(Input.mousePosition), out RaycastHit hit)) return;
        m_Navigator.SetPath(hit.point);
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
        if (!Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out m_Hit)) return;

        if (m_Hit.collider.gameObject.CompareTag("Enemy"))
        {
            BattleManager.Instance.Init(m_BattleMonster, m_Hit.collider.gameObject.GetComponent<BattleMonster>());

            m_Navigator.MoveToLocation(m_Hit.collider.gameObject.transform, true);
            m_Navigator.StopMove += BattleTransition;
        }
        else
        {
            m_Navigator.StopMove -= BattleTransition;
            m_Navigator.MoveToLocation(m_Hit.point);
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

        if (IsActive) Wake();
        else Sleep();
    }

    public void Wake()
    {
        Cursor.visible = false;

        m_InputActions.Player.Enable();

        m_InputActions.Player.Select.performed += OnSelect;
    }

    public void Sleep()
    {
        Cursor.visible = true;

        m_InputActions.Player.Select.performed -= OnSelect;
        //m_InputActions.Player.Disable();

        m_Navigator.Sleep();
    }

    public void PrepareTransitions()
    {
        TransitionManager.Instance.SubscribeToTransition(GameState.EXPLORATIONACTUAL, Enable);
        TransitionManager.Instance.SubscribeToTransition(GameState.BATTLE, Disable);
        TransitionManager.Instance.SubscribeToTransition(GameState.HOME, Disable);
    }

    public void DisableTransitions()
    {
        TransitionManager.Instance.UnsubscribeFromTransition(GameState.EXPLORATIONACTUAL, Enable);
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
