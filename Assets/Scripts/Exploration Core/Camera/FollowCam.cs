using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCam : MonoBehaviour, IManageable
{
    [SerializeField] Transform Target;
    [SerializeField] float MaxRadius;
    [SerializeField] float MinRadius;
    [SerializeField] float RotationSpeed;
    [SerializeField] float ZoomSpeed;
    [SerializeField] float SmoothTime;
    [SerializeField] float RotationAcceleration;
    [SerializeField] float MaxRotationSpeed;
    [SerializeField] float ZoomAxisDiscrepancy;
    [SerializeField] float HeightTiltThreshold;
    [SerializeField] float HeightSupplement;
    [SerializeField] float MaxHeight;

    float PivotRadius;
    float PivotAngle;
    float HeightOffset;
    float AngleIncrement;

    Vector3 Velocity;

    Vector3 IntermediaryPosition;

    PlayerController Player;

    Vector3 PivotPoint => new Vector3(Target.position.x, HeightOffset, Target.position.z);

    float CurrentHeight => transform.position.y;

    public bool IsActive { get; set; }

    void Start()
    {
        Player = FindObjectOfType<PlayerController>();
        SetActive(true);
        PrepareTransitions();
    }


    void Update()
    {
        if (!IsActive) return;

        PivotAngle += AngleIncrement * Time.deltaTime;

        if (AngleIncrement != 0)
        {
            if (Mathf.Abs(AngleIncrement) < MaxRotationSpeed)
                AngleIncrement += AngleIncrement > 0 ? RotationAcceleration * Time.deltaTime : -RotationAcceleration * Time.deltaTime;
        }

        float posX = Mathf.Cos(Mathf.Deg2Rad * PivotAngle) * PivotRadius;
        float posZ = Mathf.Sin(Mathf.Deg2Rad * PivotAngle) * PivotRadius;

        IntermediaryPosition = new Vector3(posX, 0, posZ) + PivotPoint;

        transform.position = Vector3.SmoothDamp(transform.position, IntermediaryPosition, ref Velocity, SmoothTime);

        transform.LookAt(Target);
    }

    void OnRotateCamera(float increment)
    {
        AngleIncrement = increment * RotationSpeed;
    }

    void OnZoomCamera(float increment)
    {
        if (increment > 0 && PivotRadius <= MinRadius) return;
        if (increment < 0 && (PivotRadius >= MaxRadius || CurrentHeight >= MaxHeight)) return;

        PivotRadius -= increment / Mathf.Abs(increment) * ZoomSpeed * Time.deltaTime;


        float heightIncrement = PivotRadius > HeightTiltThreshold ? ZoomAxisDiscrepancy + HeightSupplement : ZoomAxisDiscrepancy;

        HeightOffset -= increment / Mathf.Abs(increment) * ZoomSpeed * heightIncrement * Time.deltaTime;
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
        PivotRadius = (PivotPoint - transform.position).magnitude;
        PivotAngle = -Mathf.Acos(transform.position.x / PivotRadius) * Mathf.Rad2Deg;
        HeightOffset = transform.position.y;
        Player.RotateCamera += OnRotateCamera;
        Player.ZoomCamera += OnZoomCamera;
    }

    public void Sleep()
    {
        Player.RotateCamera -= OnRotateCamera;
        Player.ZoomCamera -= OnZoomCamera;

        OnRotateCamera(0);
        OnZoomCamera(0);
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
