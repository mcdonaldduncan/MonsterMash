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
    int ScaledHealth;
    int ScaledAttack;
    int ScaledDefense;
    int ScaledSpAttack;
    int ScaledSpDefense;
    int ScaledSkill;

    // Working values for battle calculations that can be scaled or changed by combat effects
    int CurrentHealth;
    int CurrentAttack;
    int CurrentDefense;
    int CurrentSpAttack;
    int CurrentSpDefense;
    int CurrentSkill;

    // Working values for resource pools
    int CurrentMana;
    int CurrentStamina;

    int CurrentManaRegen;
    int CurrentStaminaRegen;

    public BattleAction[] Actions => m_BattleActions;
    public ElementType Type => m_Type;

    // ToDo Add Effort Values

    // ToDo Add Individual Values

    Dictionary<StatType, Action<int>> StatModifiers;
    Dictionary<StatType, Func<int>> CurrentStatAccessors;
    Dictionary<StatType, Func<int>> MaxStatAccessors;
    Dictionary<StatType, Func<int>> BaseStatAccessors;
    Dictionary<ResourceType, Action<int>> ResourceModifiers;
    Dictionary<ResourceType, Func<int>> ResourceAccessors;

    private void OnEnable()
    {
        DetermineScaledValues();
        InitializeWorkingValues();
        InitializeModifiersAndAccesors();
    }

    public void ModifyStat(StatType type, int delta)
    {
        if (StatModifiers.TryGetValue(type, out var modifyAction))
        {
            modifyAction(delta);
        }
        else
        {
            Utility.LogWarning($"Invalid stat type: {type}");
        }
    }

    public int GetStat(StatType type, TypeModifier modifier = TypeModifier.CURRENT)
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
            Utility.LogWarning($"Invalid stat type: {type}");
            return -1;
        }
    }

    public void ModifyResource(ResourceType type, int delta)
    {
        if (ResourceModifiers.TryGetValue(type, out var modifyAction))
        {
            modifyAction(delta);
        }
        else
        {
            Utility.LogWarning($"Invalid resource type: {type}");
        }
    }

    public int GetResource(ResourceType type)
    {
        if (ResourceAccessors.TryGetValue(type, out var accessorFunc))
        {
            return accessorFunc();
        }
        else
        {
            Utility.LogWarning($"Invalid resource type: {type}");
            return -1;
        }
    }

    public void Regen()
    {
        // LOL
        CurrentMana = CurrentMana < m_BaseMana ? CurrentMana + CurrentManaRegen <= m_BaseMana ? CurrentMana + CurrentManaRegen : m_BaseMana : m_BaseMana;
        CurrentStamina = CurrentStamina < m_BaseStamina ? CurrentStamina + CurrentStaminaRegen <= m_BaseStamina ? CurrentStamina + CurrentStaminaRegen : m_BaseStamina : m_BaseStamina;
    }

    #region Initialization Logic

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

        CurrentManaRegen = m_BaseManaRegen;
        CurrentStaminaRegen = m_BaseStaminaRegen;
    }

    void DetermineScaledValues()
    {
        // ToDo Not sure yet how to handle level scaling, work up a model to finalize
        ScaledHealth = m_BaseHealth * m_ExpLevel;
        ScaledAttack = m_BaseAttack * m_ExpLevel;
        ScaledDefense = m_BaseDefense * m_ExpLevel;
        ScaledSpAttack = m_BaseSpAttack * m_ExpLevel;
        ScaledSpDefense = m_BaseSpDefense * m_ExpLevel;
        ScaledSkill = m_BaseSkill * m_ExpLevel;
    }


    void InitializeModifiersAndAccesors()
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

        ResourceModifiers = new Dictionary<ResourceType, Action<int>>()
        {
            // Evil laugh
            {ResourceType.MANA, value => CurrentMana = CurrentMana + value <= m_BaseMana && CurrentMana + value >= 0 ? CurrentMana + value : CurrentMana + value <= 0 ? 0 : m_BaseMana },
            {ResourceType.STAM, value => CurrentStamina =  CurrentStamina + value <= m_BaseStamina && CurrentStamina + value >= 0 ? CurrentStamina + value : CurrentStamina + value <= 0 ? 0 : m_BaseStamina }
        };

        ResourceAccessors = new Dictionary<ResourceType, Func<int>>()
        {
            {ResourceType.MANA, () => CurrentMana },
            {ResourceType.STAM, () => CurrentStamina }
        };
    }

    #endregion
}
