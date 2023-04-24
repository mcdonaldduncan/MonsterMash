using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Navigator))]
public class PlayerController : MonoBehaviour
{
    InputActions InputActions;
    Navigator Navigator;

    public delegate void CameraControlDelegate(float increment);
    public event CameraControlDelegate RotateCamera;
    public event CameraControlDelegate ZoomCamera;

    public delegate void MoveTypeDelegate(bool started);
    public event MoveTypeDelegate BattleMovePerformed;

    private void OnEnable()
    {
        Navigator = GetComponent<Navigator>();
        Navigator.SubscribeToMoveType(this);

        InputActions = new InputActions();

        Cursor.visible = false;

        InputActions.Player.Enable();


        InputActions.Player.Select.performed += OnSelect;
        InputActions.Player.Move.started += OnMove;
        InputActions.Player.Move.canceled += OnMove;

        InputActions.Player.Scroll.performed += OnScroll;
    }

    private void OnDisable()
    {
        InputActions.Player.Select.performed -= OnSelect;
        InputActions.Player.Move.started -= OnMove;
        InputActions.Player.Move.canceled -= OnMove;

        InputActions.Player.Disable();
    }

    private void Update()
    {
        if (!Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out RaycastHit hit)) return;
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
        if (!Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out RaycastHit hit)) return;

        if (hit.collider.gameObject.CompareTag("Enemy"))
        {
            BattleMovePerformed?.Invoke(true);
            Navigator.MoveToLocation(hit.collider.gameObject.transform);
        }
        else
        {
            BattleMovePerformed?.Invoke(false);
            Navigator.MoveToLocation(hit.point);
        }
    }
}
