using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AiDecisionController
{
    BattleAction m_DefaultAction;

    public BattleAction BestMove(BattleMonster monster)
    {
        if (m_DefaultAction == null)
        {
            m_DefaultAction = ScriptableObject.CreateInstance<BasicAttack>();
        }

        // If the actions are null or empty, return default
        if (monster.Actions == null || monster.Actions.Length == 0) return m_DefaultAction;

        // If the monster has a single move, return that move
        if (monster.Actions.Length == 1) return monster.Actions[0];


        if(monster.GetStat(StatType.HEALTH) > 0)
        {

        }





        return monster.Actions[0];
    }

}
