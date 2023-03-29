using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class BattleMonster : MonoBehaviour
{
    [Header("Monster Level")]
    [Range(1, 100)]
    [SerializeField] int m_ExpLevel = 1;

    [Header("Base Stats")]
    [SerializeField] int m_BaseHealth;
    [SerializeField] int m_BaseAttack;
    [SerializeField] int m_BaseDefense;
    [SerializeField] int m_BaseSpAttack;
    [SerializeField] int m_BaseSpDefense;
    [SerializeField] int m_BaseSkill;

    [Header("Base Resorce Pools")]
    [SerializeField] int m_BaseMana;
    [SerializeField] int m_BaseStamina;

    [Header("Base Resorce Regen")]
    [SerializeField] int m_BaseManaRegen;
    [SerializeField] int m_BaseStaminaRegen;
    
    [Header("Move Set")]
    [SerializeField] BattleAction[] m_BattleActions;

    // Values determined by base and level
    int m_ScaledHealth;
    int m_ScaledAttack;
    int m_ScaledDefense;
    int m_ScaledSpAttack;
    int m_ScaledSpDefense;
    int m_ScaledSkill;

    // Working values for battle calculations that can be scaled or changed by combat effects
    [NonSerialized] public int CurrentHealth;
    [NonSerialized] public int CurrentAttack;
    [NonSerialized] public int CurrentDefense;
    [NonSerialized] public int CurrentSpAttack;
    [NonSerialized] public int CurrentSpDefense;
    [NonSerialized] public int CurrentSkill;

    // Working values for resource pools
    [NonSerialized] public int CurrentMana;
    [NonSerialized] public int CurrentStamina;


    Dictionary<StatType, Action<int>> StatLookup;

    private void Start()
    {
        StatLookup = new Dictionary<StatType, Action<int>>()
        {
            { StatType.HEALTH, value => CurrentHealth += value },
            { StatType.ATTACK, value => CurrentAttack += value },
            { StatType.DEFENSE, value => CurrentDefense += value },
            { StatType.SPATTACK, value => CurrentSpAttack += value },
            { StatType.SPDEFENSE, value => CurrentSpDefense += value },
            { StatType.SKILL, value => CurrentSkill += value }
        };
    }

    void DetermineScaledValues()
    {
        // Not sure how I will handle level scaling yet, will need to work up an excel model to finalize
        m_ScaledHealth = 100;
        m_ScaledAttack = 10;
        m_ScaledDefense = 10;
        m_ScaledSpAttack = 10;
        m_ScaledSpDefense = 10;
        m_ScaledSkill = 10;
    }

    public void InitializeWorkingValues()
    {
        CurrentHealth = m_ScaledHealth;
        CurrentAttack = m_ScaledAttack;
        CurrentDefense = m_ScaledDefense;
        CurrentSpAttack = m_ScaledSpAttack;
        CurrentSpDefense = m_ScaledSpDefense;
        CurrentSkill = m_ScaledSkill;

        CurrentMana = m_BaseMana;
        CurrentStamina = m_BaseStamina;
    }

    public void AlterStat(StatType type, int delta)
    {
        if (StatLookup.TryGetValue(type, out var modifyAction))
        {
            modifyAction(delta);
        }
        else
        {
            Debug.LogWarning($"Invalid stat type: {type}");
        }
    }

}

public enum StatType
{
    HEALTH,
    ATTACK,
    DEFENSE,
    SPATTACK,
    SPDEFENSE,
    SKILL
}