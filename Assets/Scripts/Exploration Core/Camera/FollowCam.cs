using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCam : MonoBehaviour, IManageable
{
    [SerializeField] Transform Target;
    [SerializeField] float m_MaxRadius;
    [SerializeField] float m_MinRadius;
    [SerializeField] float m_StartRadius;
    [SerializeField] float m_RotationSpeed;
    [SerializeField] float m_ZoomSpeed;
    [SerializeField] float m_SmoothTime;
    [SerializeField] float m_RotationAcceleration;
    [SerializeField] float m_MaxRotationSpeed;
    [SerializeField] float m_ZoomAxisDiscrepancy;
    [SerializeField] float m_HeightTiltThreshold;
    [SerializeField] float m_HeightSupplement;
    [SerializeField] float m_MaxHeight;
    [SerializeField] float m_MinHeight;
    [SerializeField] float m_StartHeight;

    float m_PivotRadius;
    float m_PivotAngle;
    float m_HeightOffset;
    float m_AngleIncrement;

    Vector3 m_Velocity;

    Vector3 m_IntermediaryPosition;

    PlayerController m_Player;
    Transform m_Transform;

    Vector3 PivotPoint => new(Target.position.x, Target.position.y + m_HeightOffset, Target.position.z);

    public bool IsActive { get; set; }
    public bool IsTargetMoving { get; set; }

    private void Start()
    {
        m_Transform = transform;
        m_Player = FindObjectOfType<PlayerController>();
        SetActive(true);
        //PrepareTransitions(); - Using this cam in all game states at the moment, will change later
    }

    private void Update()
    {
        if (!IsActive) return;

        m_PivotAngle += m_AngleIncrement * Time.deltaTime;

        if (m_AngleIncrement != 0)
        {
            if (Mathf.Abs(m_AngleIncrement) < m_MaxRotationSpeed)
                m_AngleIncrement += m_AngleIncrement > 0 ? m_RotationAcceleration * Time.deltaTime : -m_RotationAcceleration * Time.deltaTime;
        }
        else if (IsTargetMoving)
        {
            Vector3 directionToTarget = PivotPoint - m_IntermediaryPosition;
            float desiredAngle = Mathf.Atan2(directionToTarget.z, directionToTarget.x) * Mathf.Rad2Deg + 180f;
            if (desiredAngle < 0) desiredAngle += 360f;
            m_PivotAngle = Mathf.MoveTowardsAngle(m_PivotAngle, desiredAngle, m_RotationSpeed * Time.deltaTime);
        }

        float posX = Mathf.Cos(Mathf.Deg2Rad * m_PivotAngle) * m_PivotRadius;
        float posZ = Mathf.Sin(Mathf.Deg2Rad * m_PivotAngle) * m_PivotRadius;

        m_IntermediaryPosition = new Vector3(posX, 0, posZ) + PivotPoint;

        m_Transform.position = Vector3.SmoothDamp(m_Transform.position, m_IntermediaryPosition, ref m_Velocity, m_SmoothTime);

        m_Transform.LookAt(Target);
    }

    private void OnRotateCamera(float increment)
    {
        m_AngleIncrement = increment * m_RotationSpeed;
    }

    private void OnZoomCamera(float increment)
    {
        if ((increment < 0 && m_PivotRadius <= m_MaxRadius) || (increment > 0 && m_PivotRadius >= m_MinRadius))
        {
            m_PivotRadius -= increment / Mathf.Abs(increment) * m_ZoomSpeed * Time.deltaTime;
        }

        if ((increment > 0 && m_HeightOffset >= m_MinHeight) || (increment < 0 && m_HeightOffset <= m_MaxHeight))
        {
            float heightIncrement = m_HeightOffset > m_HeightTiltThreshold ? m_ZoomAxisDiscrepancy + m_HeightSupplement : m_ZoomAxisDiscrepancy;
            m_HeightOffset -= increment / Mathf.Abs(increment) * m_ZoomSpeed * heightIncrement * Time.deltaTime;
        }

        m_HeightOffset = Mathf.Clamp(m_HeightOffset, m_MinHeight, m_MaxHeight);
        m_PivotRadius = Mathf.Clamp(m_PivotRadius, m_MinRadius, m_MaxRadius);
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
        m_HeightOffset = m_HeightOffset <= m_MinHeight ? m_StartHeight : m_HeightOffset;
        if (m_IntermediaryPosition.magnitude == 0)
        {
            var startPosition = PivotPoint - (Target.forward * m_StartRadius);
            m_Transform.position = startPosition;
            m_IntermediaryPosition = startPosition;
        }

        var currentDir = PivotPoint - m_Transform.position;
        
        m_PivotRadius = currentDir.magnitude;
        m_PivotAngle = Mathf.Atan2(currentDir.z, currentDir.x) * Mathf.Rad2Deg + 180f;
        if (m_PivotAngle < 0) m_PivotAngle += 360f;

        m_Player.RotateCamera += OnRotateCamera;
        m_Player.ZoomCamera += OnZoomCamera;
        m_Player.SetMoving += OnSetMoving;
    }

    public void Sleep()
    {
        m_Player.RotateCamera -= OnRotateCamera;
        m_Player.ZoomCamera -= OnZoomCamera;
        m_Player.SetMoving -= OnSetMoving; // make sure this doesnt continue movement after sleep

        OnRotateCamera(0);
        OnZoomCamera(0);
    }

    public void PrepareTransitions()
    {
        TransitionController.Instance.SubscribeToTransition(GameState.EXPLORATION, Enable);
        TransitionController.Instance.SubscribeToTransition(GameState.BATTLE, Disable);
        TransitionController.Instance.SubscribeToTransition(GameState.HOME, Disable);
    }

    public void DisableTransitions()
    {
        TransitionController.Instance.UnsubscribeFromTransition(GameState.EXPLORATION, Enable);
        TransitionController.Instance.UnsubscribeFromTransition(GameState.BATTLE, Disable);
        TransitionController.Instance.UnsubscribeFromTransition(GameState.HOME, Disable);
    }

    public void Enable()
    {
        SetActive(true);
    }

    public void Disable()
    {
        SetActive(false);
    }

    private void OnSetMoving(bool newState)
    {
        IsTargetMoving = newState;
    }
}
