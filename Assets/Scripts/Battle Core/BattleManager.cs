using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleManager<T, U> where T : BattleMonster where U : BattleMonster
{
    BattleMonster m_Player;
    BattleMonster m_Enemy;

    public BattleManager(BattleMonster player, BattleMonster enemy)
    {
        m_Player = player;
        m_Enemy = enemy;
    }
    

    public void Init(T player, U enemy)
    {
        m_Player = player;
        m_Enemy = enemy;
    }
}
