using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public abstract class BattleAction : ScriptableObject, IAction
{
    [SerializeField] string m_ActionName;
    [SerializeField] int m_Cost;
    [SerializeField] int m_Power;
    [SerializeField] ResourceType m_ResourceType;
    [SerializeField] StatType m_ActionModifier;
    [SerializeField] ElementType m_ActionType;

    public string Name => m_ActionName;
    public int Cost => m_Cost;
    public int Power => m_Power;
    public StatType Modifier => m_ActionModifier;
    public ResourceType ResourceType => m_ResourceType;
    public ElementType Type => m_ActionType;

    public virtual void InvokeAction(BattleMonster invoker, BattleMonster target) { }
}
