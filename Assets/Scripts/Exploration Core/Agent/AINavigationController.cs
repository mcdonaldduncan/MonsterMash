using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(Navigator))]
public class AINavigationController : MonoBehaviour
{
    [SerializeField] AgentState m_StartingState;
    [SerializeField] float m_WanderInterval;
    [SerializeField] float m_MaxWanderDistance;

    Navigator m_Navigator;
    Coroutine m_WanderRoutine;

    AgentState m_AgentState;

    Vector3 m_StartingPosition;

    float m_LastWanderTime;
    float m_CurrentWanderInterval;

    delegate void AgentDecisionDelegate();

    Dictionary<AgentState, AgentDecisionDelegate> m_AgentActions;

    bool shouldWander => Time.time >= m_CurrentWanderInterval + m_LastWanderTime;

    void Start()
    {
        SetupActions();

        m_Navigator = GetComponent<Navigator>();
        m_AgentState = m_StartingState;
        m_StartingPosition = transform.position;

        PerformAction();
    }

    void PerformAction()
    {
        if (m_AgentActions.TryGetValue(m_AgentState, out AgentDecisionDelegate action))
        {
            action?.Invoke();
        }
        else
        {
            Utility.LogWarning($"AgentState {m_AgentState} has no actions defined");
        }
    }

    #region Action Definitions

    void Wander()
    {
        if (m_WanderRoutine != null) StopCoroutine(m_WanderRoutine);
        m_WanderRoutine = StartCoroutine(WanderRoutine());
    }

    IEnumerator WanderRoutine()
    {
        while (!shouldWander)
        {
            yield return null;
        }

        m_Navigator.StopMove += PerformAction;
        m_Navigator.MoveToLocation(RandomPosInSphere(transform.position, m_MaxWanderDistance, NavMesh.AllAreas) ?? m_StartingPosition);
        

        m_LastWanderTime = Time.time;
        m_CurrentWanderInterval = Random.Range(0, m_WanderInterval);
    }

    Vector3? RandomPosInSphere(Vector3 origin, float distance, LayerMask layerMask)
    {
        Vector3 randomPosition = Random.insideUnitSphere * distance;
        if (NavMesh.SamplePosition(randomPosition + origin, out NavMeshHit navHit, distance, layerMask))
        {
            return navHit.position;
        }
        else
        {
            return null;
        }
    }

    #endregion

    #region Action Setup

    void SetupActions()
    {
        m_AgentActions = new Dictionary<AgentState, AgentDecisionDelegate>();

        m_AgentActions.Add(AgentState.WANDER, Wander);
    }

    #endregion
}
