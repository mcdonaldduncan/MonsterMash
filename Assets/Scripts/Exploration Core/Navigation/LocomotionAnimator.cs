using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(Navigator))]
public class LocomotionAnimator : MonoBehaviour
{
    Animator Animator;
    Navigator Navigator;

    const string IsMoving = "IsMoving";
    const string IsAttacking = "IsAttacking";

    void Start()
    {
        Animator = GetComponentInChildren<Animator>();
        Navigator = GetComponent<Navigator>();

        Navigator.StartMove += OnStartMove;
        Navigator.StopMove += OnStopMove;
    }


    public void OnStartMove()
    {
        Animator.SetBool(IsMoving, true);
    }

    public void OnStopMove()
    {
        Animator.SetBool(IsMoving, false);
    }
}
