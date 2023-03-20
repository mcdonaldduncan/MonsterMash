using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCam : MonoBehaviour
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

    float PivotRadius;
    float PivotAngle;
    float HeightOffset;

    Vector3 Velocity;

    Vector3 IntermediaryPosition;

    PlayerController Player;

    Vector3 PivotPoint => new Vector3(Target.position.x, HeightOffset, Target.position.z);

    float AngleIncrement;

    void Start()
    {
        Player = FindObjectOfType<PlayerController>();
        PivotRadius = (PivotPoint - transform.position).magnitude;

        HeightOffset = transform.position.y;
        Player.RotateCamera += OnRotateCamera;
        Player.ZoomCamera += OnZoomCamera;
    }


    void Update()
    {
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
        if (increment < 0 && PivotRadius >= MaxRadius) return;

        PivotRadius -= increment / Mathf.Abs(increment) * ZoomSpeed * Time.deltaTime;
        HeightOffset -= increment / Mathf.Abs(increment) * ZoomSpeed * ZoomAxisDiscrepancy * Time.deltaTime;
    }
}
