using Assets.Scripts.Common;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(Navigator))]
public class NavigationAnimator : MonoBehaviour
{
    Animator Animator;
    Navigator Navigator;

    void Start()
    {
        Animator = GetComponentInChildren<Animator>();
        Navigator = GetComponent<Navigator>();

        Navigator.StartMove += OnStartMove;
        Navigator.StopMove += OnStopMove;
    }

    public void OnStartMove()
    {
        Animator.SetBool(Constants.IsMoving, true);
    }

    public void OnStopMove()
    {
        Animator.SetBool(Constants.IsMoving, false);
    }
}
