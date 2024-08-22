using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : Singleton<CameraController>
{
    Camera m_Camera;
    CinemachineBrain m_Brain;
    CinemachineTargetGroup m_TargetGroup;
    CinemachineVirtualCamera m_VirtualCamera;

    public override void Awake()
    {
        base.Awake();

        m_Camera = Camera.main;
        m_Brain = m_Camera.GetComponent<CinemachineBrain>();
        m_VirtualCamera = m_Camera.GetComponent<CinemachineVirtualCamera>();

        m_TargetGroup = FindObjectOfType<CinemachineTargetGroup>();
    }

    private void Start()
    {
        //TransitionManager.Instance.SubscribeToTransition(GameState.BATTLEACTUAL, OnBattleActual);
    }

    public void OnBattleActual()
    {
        m_Brain.enabled = true;
    }

    public void SetPosition(Vector3 position)
    {
        transform.position = position;
    }

    public void SetTargets(Transform player, Transform enemy)
    {
        m_TargetGroup.m_Targets = new CinemachineTargetGroup.Target[0];
        m_TargetGroup.AddMember(player, 1, 1f);
        m_TargetGroup.AddMember(enemy, 1, 1f);
    }
}
