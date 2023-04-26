using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BattleMonster))]
public class AIController : MonoBehaviour
{
    BattleMonster m_Monster;
    BattleManager<BattleMonster, BattleMonster> m_CurrentManager;

    // Default action if monster does not have actions (Think flail)
    BattleAction m_DefaultAction;

    private void OnEnable()
    {
        m_Monster = GetComponent<BattleMonster>();
    }

    private BattleAction BestMove()
    {
        // If the actions are null or empty, return default
        if (m_Monster.Actions == null || m_Monster.Actions.Length == 0) return m_DefaultAction;

        // If the monster has a single move, return that move
        if (m_Monster.Actions.Length == 1) return m_Monster.Actions[0];


        if(m_Monster.GetStat(StatType.HEALTH) > 0)
        {

        }





        return m_Monster.Actions[0];
    }

}
