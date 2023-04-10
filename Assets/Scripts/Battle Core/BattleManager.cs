using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleManager<T, U> : MonoBehaviour where T : BattleMonster where U : BattleMonster
{
    BattleMonster m_Player;
    BattleMonster m_Enemy;


    void Start()
    {
        
    }

    void Update()
    {
        
    }

    public void Init(T player, U enemy)
    {
        m_Player = player;
        m_Enemy = enemy;
    }
}
