using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class Navigator : MonoBehaviour
{
    [SerializeField] GameObject DestinationTrigger;

    NavMeshAgent Agent;
    NavMeshPath Path;
    
    Transform DestinationTransform;

    int TriggerID;

    public delegate void MovementStateDelegate();
    public MovementStateDelegate StartMove;
    public MovementStateDelegate StopMove;

    public delegate void PathStateDelegate(Vector3 location);
    public PathStateDelegate PathProcessed;
    public PathStateDelegate PathPending;

    private void Awake()
    {
        Agent = GetComponent<NavMeshAgent>();
        Path = new NavMeshPath();
    }

    private void OnEnable()
    {
        GameObject temp = Instantiate(DestinationTrigger, transform.position, Quaternion.identity);
        DestinationTransform = temp.transform;
        TriggerID = temp.GetInstanceID();
    }


    public void MoveToLocation(Vector3 location)
    {
        Agent.SetDestination(location);
        StartCoroutine(WaitForPathProcessing());
        StartMove?.Invoke();
    }

    public void MoveToLocation(Transform target)
    {
        MoveToLocation(target.position);
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
        if (!(other.gameObject.GetInstanceID() == TriggerID)) return;
        StopMove?.Invoke();
    }

    IEnumerator WaitForPathProcessing()
    {
        while (Agent.pathPending)
        {
            yield return null;
        }

        DestinationTransform.position = Agent.destination;
        PathProcessed?.Invoke(Agent.destination);
    }

    public void ConfirmPath()
    {
        Agent.SetPath(Path);
        StartCoroutine(WaitForPathProcessing());
        StartMove?.Invoke();
    }


    public void SetPath(Vector3 location)
    {
        if (Agent.CalculatePath(location, Path))
        {
            PathPending?.Invoke(Path.corners[Path.corners.Length - 1]);
        }
    }
}
