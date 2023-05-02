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

    NavMeshAgent Agent;
    NavMeshPath Path;
    
    Transform DestinationTransform;
    Transform TargetTransform;

    Coroutine CombatMoveRoutine;
    Coroutine PathPendingRoutine;

    int TriggerID;

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
        Agent = GetComponent<NavMeshAgent>();
        Path = new NavMeshPath();

        GetComponent<Rigidbody>().freezeRotation = true;
    }

    private void OnEnable()
    {
        GameObject temp = Instantiate(DestinationTrigger, transform.position, Quaternion.identity);
        DestinationTransform = temp.transform;
        TriggerID = temp.GetInstanceID();
    }

    public void MoveToLocation(Vector3 location, bool combatMove = false)
    {
        if (Agent.isStopped) Agent.isStopped = false;
        if (OnCombatMove != combatMove) OnCombatMove = combatMove;
        Agent.SetDestination(location);
        if (PathPendingRoutine != null) StopCoroutine(PathPendingRoutine);
        PathPendingRoutine = StartCoroutine(WaitForPathProcessing());
        StartMove?.Invoke();
    }

    public void MoveToLocation(Transform target, bool combatMove = false)
    {
        TargetTransform = target;
        MoveToLocation(target.position, combatMove);
    }

    public void SetLocation(Vector3 location)
    {
        Agent.Warp(location);
    }

    public void SetLocation(Transform target)
    {
        Agent.Warp(target.position);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetInstanceID() != TriggerID) return;

        if (OnCombatMove && CombatMoveRoutine != null)
        {
            OnCombatMove = false;
            StopCoroutine(CombatMoveRoutine);
            CombatMoveRoutine = null;
        }

        StopMove?.Invoke();
    }

    IEnumerator MaintainCombatMove()
    {
        while (OnCombatMove)
        {
            yield return null;
            Agent.SetDestination(TargetTransform.position);
            DestinationTransform.position = Agent.destination;
            PathMaintain?.Invoke(TargetTransform.position);
        }
    }

    IEnumerator WaitForPathProcessing()
    {
        while (Agent.pathPending)
        {
            yield return null;
        }

        DestinationTransform.position = Agent.destination;
        PathProcessed?.Invoke(Agent.destination);

        if (OnCombatMove && CombatMoveRoutine == null)
        {
            CombatMoveRoutine = StartCoroutine(MaintainCombatMove());
        }
        else if (!OnCombatMove && CombatMoveRoutine != null)
        {
            StopCoroutine(CombatMoveRoutine);
            CombatMoveRoutine = null;
        }
    }

    public void SetPath(Vector3 location)
    {
        if (Agent.CalculatePath(location, Path))
        {
            PathPending?.Invoke(Path.corners[Path.corners.Length - 1]);
        }
    }

    public void Sleep()
    {
        Agent.isStopped = true;
        StopMove?.Invoke();
    }
}
