using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Navigator))]
public class AINavigationController : MonoBehaviour
{
    [SerializeField] AgentState m_StartingState;
    [SerializeField] float m_WanderInterval;
    [SerializeField] float m_MaxWanderDistance;



    AgentState m_AgentState;

    delegate void AgentDecisionDelegate();

    Dictionary<AgentState, AgentDecisionDelegate> AgentActions;




    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void Wander()
    {

    }
}
