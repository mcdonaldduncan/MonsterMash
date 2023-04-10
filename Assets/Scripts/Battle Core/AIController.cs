using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BattleMonster))]
public class AIController : MonoBehaviour
{
    BattleMonster m_Monster;
    BattleManager<BattleMonster, BattleMonster> m_CurrentManager;


    private void OnEnable()
    {
        m_Monster = GetComponent<BattleMonster>();
    }

    private BattleAction BestMove()
    {
        if (m_Monster.Actions == null || m_Monster.Actions.Length == 0) return null;

        if (m_Monster.Actions.Length == 1) return m_Monster.Actions[0];








        return m_Monster.Actions[0];
    }

}
