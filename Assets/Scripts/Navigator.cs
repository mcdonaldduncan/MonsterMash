using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class Navigator : MonoBehaviour
{
    NavMeshAgent Agent;

    private void Awake()
    {
        Agent = GetComponent<NavMeshAgent>();
    }

    
    public void MoveToLocation(Vector3 location)
    {
        Agent.SetDestination(location);
    }

    public void MoveToLocation(Transform target)
    {
        Agent.SetDestination(target.position);
    }



    public void SetLocation(Vector3 location)
    {
        Agent.Warp(location);
    }

    public void SetLocation(Transform target)
    {
        Agent.Warp(target.position);
    }
}
