using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Navigator))]
public class PlayerController : MonoBehaviour
{
    [SerializeField] GameObject m_PlayerCreature;

    InputActions InputActions;
    Navigator Navigator;

    private void Awake()
    {
        InputActions = new InputActions();

        InputActions.Player.Enable();

        InputActions.Player.Select.performed += OnSelect;
    }

    private void OnEnable()
    {
        Navigator = GetComponent<Navigator>();
    }

    private void OnDisable()
    {
        InputActions.Player.Select.performed -= OnSelect;

        InputActions.Player.Disable();
    }

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    public void OnSelect(InputAction.CallbackContext context)
    {
        if (!Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out RaycastHit hit)) return;


        Navigator.MoveToLocation(hit.point);
    }
}
