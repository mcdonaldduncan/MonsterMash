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

    [Header("Monster Type")]
    [SerializeField] ElementType m_Type;

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
    public int ScaledHealth;
    public int ScaledAttack;
    public int ScaledDefense;
    public int ScaledSpAttack;
    public int ScaledSpDefense;
    public int ScaledSkill;

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

    public BattleAction[] Actions => m_BattleActions;
    public ElementType Type => m_Type;

    // Add Effort Values

    // Add Individual Values

    Dictionary<StatType, Action<int>> StatModifiers;
    Dictionary<StatType, Func<int>> CurrentStatAccessors;
    Dictionary<StatType, Func<int>> MaxStatAccessors;
    Dictionary<StatType, Func<int>> BaseStatAccessors;

    private void Start()
    {
        StatModifiers = new Dictionary<StatType, Action<int>>()
        {
            { StatType.HEALTH, value => CurrentHealth += value },
            { StatType.ATTACK, value => CurrentAttack += value },
            { StatType.DEFENSE, value => CurrentDefense += value },
            { StatType.SPATTACK, value => CurrentSpAttack += value },
            { StatType.SPDEFENSE, value => CurrentSpDefense += value },
            { StatType.SKILL, value => CurrentSkill += value }
        };

        CurrentStatAccessors = new Dictionary<StatType, Func<int>>()
        {
            { StatType.HEALTH, () => CurrentHealth  },
            { StatType.ATTACK, () => CurrentAttack },
            { StatType.DEFENSE, () => CurrentDefense },
            { StatType.SPATTACK, () => CurrentSpAttack },
            { StatType.SPDEFENSE, () => CurrentSpDefense },
            { StatType.SKILL, () => CurrentSkill }
        };

        MaxStatAccessors = new Dictionary<StatType, Func<int>>()
        {
            { StatType.HEALTH, () => ScaledHealth  },
            { StatType.ATTACK, () => ScaledAttack },
            { StatType.DEFENSE, () => ScaledDefense },
            { StatType.SPATTACK, () => ScaledSpAttack },
            { StatType.SPDEFENSE, () => ScaledSpDefense },
            { StatType.SKILL, () => ScaledSkill }
        };

        BaseStatAccessors = new Dictionary<StatType, Func<int>>()
        {
            { StatType.HEALTH, () => m_BaseHealth  },
            { StatType.ATTACK, () => m_BaseAttack },
            { StatType.DEFENSE, () => m_BaseDefense },
            { StatType.SPATTACK, () => m_BaseSpAttack },
            { StatType.SPDEFENSE, () => m_BaseSpDefense },
            { StatType.SKILL, () => m_BaseSkill }
        };
    }

    public void AlterStat(StatType type, int delta)
    {
        if (StatModifiers.TryGetValue(type, out var modifyAction))
        {
            modifyAction(delta);
        }
        else
        {
            Debug.LogWarning($"Invalid stat type: {type}");
        }
    }

    public int GetStat(StatType type, TypeModifier modifier)
    {
        var accessors = modifier == TypeModifier.CURRENT ? CurrentStatAccessors 
                        : modifier == TypeModifier.MAX ? MaxStatAccessors 
                        : BaseStatAccessors;

        if (accessors.TryGetValue(type, out var accessorFunc))
        {
            return accessorFunc();
        }
        else
        {
            Debug.LogWarning($"Invalid stat type: {type}");
            return -1;
        }
    }


    void InitializeWorkingValues()
    {
        CurrentHealth = ScaledHealth;
        CurrentAttack = ScaledAttack;
        CurrentDefense = ScaledDefense;
        CurrentSpAttack = ScaledSpAttack;
        CurrentSpDefense = ScaledSpDefense;
        CurrentSkill = ScaledSkill;

        CurrentMana = m_BaseMana;
        CurrentStamina = m_BaseStamina;
    }

    void DetermineScaledValues()
    {
        // Not sure how I will handle level scaling yet, will need to work up an excel model to finalize
        ScaledHealth = 100;
        ScaledAttack = 10;
        ScaledDefense = 10;
        ScaledSpAttack = 10;
        ScaledSpDefense = 10;
        ScaledSkill = 10;
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

public enum TypeModifier
{
    CURRENT,
    MAX,
    BASE
}