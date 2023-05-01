using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BattleAction : IAction
{
    [SerializeField] string m_Name;
    [SerializeField] int m_Cost;
    [SerializeField] int m_Power;
    [SerializeField] StatType m_ActionModifier;
    [SerializeField] ElementType m_ActionType;

    public string Name => m_Name;
    public int Cost => m_Cost;
    public int Power => m_Power;
    public StatType Modifier => m_ActionModifier;
    public ElementType Type => m_ActionType;
    

    public virtual void Invoke(BattleMonster invoker, BattleMonster target) { }
}
