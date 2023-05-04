using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Rigidbody))]
public class Navigator : MonoBehaviour
{
    [SerializeField] GameObject DestinationTrigger;

    NavMeshAgent m_Agent;
    NavMeshPath m_Path;

    GameObject m_DestinationTrigger;

    Transform m_DestinationTriggerTransform;
    Transform m_TargetTransform;

    Coroutine m_CombatMoveRoutine;
    Coroutine m_PathPendingRoutine;

    int m_TriggerID;

    public delegate void MovementStateDelegate();
    public event MovementStateDelegate StartMove;
    public event MovementStateDelegate StopMove;

    public delegate void PathStateDelegate(Vector3 location);
    public event PathStateDelegate PathProcessed;
    public event PathStateDelegate PathPending;
    public event PathStateDelegate PathMaintain;

    [NonSerialized] public bool OnCombatMove;

    private void Awake()
    {
        m_Agent = GetComponent<NavMeshAgent>();
        m_Path = new NavMeshPath();

        GetComponent<Rigidbody>().freezeRotation = true;
    }

    private void OnEnable()
    {
        m_DestinationTrigger = Instantiate(DestinationTrigger, transform.position, Quaternion.identity);
        m_DestinationTriggerTransform = m_DestinationTrigger.transform;
        m_TriggerID = m_DestinationTrigger.GetInstanceID();
    }

    public void MoveToLocation(Vector3 location, bool combatMove = false)
    {
        if (OnCombatMove != combatMove) OnCombatMove = combatMove;
        if (m_PathPendingRoutine != null) StopCoroutine(m_PathPendingRoutine);


        StartMove?.Invoke();
        m_DestinationTrigger.SetActive(false);
        m_Agent.SetDestination(location);
        m_PathPendingRoutine = StartCoroutine(WaitForPathProcessing());
    }

    public void MoveToLocation(Transform target, bool combatMove = false)
    {
        m_TargetTransform = target;
        MoveToLocation(target.position, combatMove);
    }

    public void SetLocation(Vector3 location)
    {
        m_Agent.Warp(location);
    }

    public void SetLocation(Transform target)
    {
        m_Agent.Warp(target.position);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetInstanceID() != m_TriggerID) return;

        if (OnCombatMove && m_CombatMoveRoutine != null)
        {
            OnCombatMove = false;
            StopCoroutine(m_CombatMoveRoutine);
            m_CombatMoveRoutine = null;
        }

        StopMove?.Invoke();
    }

    IEnumerator MaintainCombatMove()
    {
        while (OnCombatMove)
        {
            yield return null;
            m_Agent.SetDestination(m_TargetTransform.position);
            m_DestinationTriggerTransform.position = m_Agent.destination;
            PathMaintain?.Invoke(m_TargetTransform.position);
        }
    }

    IEnumerator WaitForPathProcessing()
    {
        while (m_Agent.pathPending)
        {
            yield return null;
        }

        m_DestinationTriggerTransform.position = m_Agent.destination;
        m_DestinationTrigger.SetActive(true);
        PathProcessed?.Invoke(m_Agent.destination);

        if (OnCombatMove && m_CombatMoveRoutine == null)
        {
            m_CombatMoveRoutine = StartCoroutine(MaintainCombatMove());
        }
        else if (!OnCombatMove && m_CombatMoveRoutine != null)
        {
            StopCoroutine(m_CombatMoveRoutine);
            m_CombatMoveRoutine = null;
        }
    }

    public void SetPath(Vector3 location)
    {
        if (m_Agent.CalculatePath(location, m_Path))
        {
            PathPending?.Invoke(m_Path.corners[m_Path.corners.Length - 1]);
        }
    }

    public void Sleep()
    {
        m_Agent.ResetPath();
        StopMove?.Invoke();
    }
}
