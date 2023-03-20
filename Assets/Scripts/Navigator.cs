using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class Navigator : MonoBehaviour
{
    [SerializeField] GameObject DestinationTrigger;

    NavMeshAgent Agent;

    Transform DestinationTransform;

    int TriggerID;

    public delegate void MovementStateDelegate();
    public MovementStateDelegate StartMove;
    public MovementStateDelegate StopMove;

    private void Awake()
    {
        Agent = GetComponent<NavMeshAgent>();
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
        StartCoroutine(SetTriggerPosition());
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


    private void OnTriggerEnter(Collider other)
    {
        if (!(other.gameObject.GetInstanceID() == TriggerID)) return;
        StopMove?.Invoke();
    }

    private IEnumerator SetTriggerPosition()
    {
        while (Agent.pathPending)
        {
            yield return null;
        }

        DestinationTransform.position = Agent.destination;
    }
}
