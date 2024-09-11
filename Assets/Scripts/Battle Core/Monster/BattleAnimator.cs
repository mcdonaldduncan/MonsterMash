using Assets.Scripts.Common;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(BattleMonster))]
public class BattleAnimator : MonoBehaviour
{
    Animator m_Animator;
    BattleMonster m_Monster;

    Dictionary<string, Action> m_BooleanResets;

    void Start()
    {
        m_BooleanResets = new Dictionary<string, Action>();
        m_Monster = GetComponent<BattleMonster>();
        m_Animator = GetComponentInChildren<Animator>();

        foreach (var p in m_Animator.parameters.Where(x => x.type == AnimatorControllerParameterType.Bool))
        {
            m_BooleanResets.Add(p.name, () => m_Animator.SetBool(p.name, false));
        }

        var actions = m_Monster.Actions;
        for (int i = 0; i < actions.Length; i++)
        {
            var actionIndex = i;
            actions[actionIndex].Perform += OnPerform;
        }

        m_Monster.StatChanged += OnStatChanged;
    }

    public void OnPerform(ResourceType type)
    {
        m_Animator.SetTrigger(type == ResourceType.STAM ? Constants.Attack : Constants.Cast);
    }

    public void OnBattle()
    {
        // sub this to battle transition
        // if monster == enemy - sub ondeath to victory
    }

    public void OnStatChanged(StatType type, bool isPositive)
    {
        switch (type)
        {
            case StatType.HEALTH:
                m_Animator.SetTrigger(isPositive ? Constants.Heal : Constants.TakeDamage);
                break;
            default:
                m_Animator.SetTrigger(isPositive ? Constants.Buff : Constants.DeBuff);
                break;
        }
    }

    public void OnDeath()
    {
        m_Animator.SetBool(Constants.IsDead, true);
    }

    private void FlipBool(string key)
    {
        var duration = m_Animator.GetAnimatorTransitionInfo(0).duration;

        if (m_BooleanResets.TryGetValue(key, out var action))
        {
            Invoke(nameof(action), duration);
        }
    }
}
