using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(Navigator))]
[RequireComponent(typeof(NavigationAnimator))]
[RequireComponent(typeof(BattleAnimator))]
[RequireComponent(typeof(BattleVFXController))]
public class BattleMonster : MonoBehaviour
{
    [Header("Name")]
    [SerializeField] string m_MonsterName;

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

    [Header("Base Resource Pools")]
    [SerializeField] int m_BaseMana;
    [SerializeField] int m_BaseStamina;

    [Header("Base Resource Regen")]
    [SerializeField] int m_BaseManaRegen;
    [SerializeField] int m_BaseStaminaRegen;

    [Header("")]
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
    public string DisplayName => m_MonsterName;

    [NonSerialized] public int Id;
    [NonSerialized] public Collider Collider;
    [NonSerialized] public BattleAnimator BattleAnimator;

    // ToDo Add Effort Values

    // ToDo Add Individual Values

    Dictionary<StatType, Action<int>> StatModifiers;
    Dictionary<StatType, Func<int>> CurrentStatAccessors;
    Dictionary<StatType, Func<int>> InitialStatAccessors;
    Dictionary<StatType, Func<int>> BaseStatAccessors;
    Dictionary<ResourceType, Action<int>> ResourceModifiers;
    Dictionary<ResourceType, Func<int>> ResourceAccessors;

    public event Action<StatType, bool> StatChanged;

    private void Awake()
    {
        Id = IdHelper.GetNextID();
        Collider = GetComponent<Collider>();
        BattleAnimator = GetComponent<BattleAnimator>();
    }

    private void OnEnable()
    {
        DetermineScaledValues();
        InitializeWorkingValues();
        InitializeModifiersAndAccesors();
        InitializeUniqueActionInstances();
    }

    public void InitializeUniqueActionInstances()
    {
        var uniqueActions = new BattleAction[m_BattleActions.Length];

        for (int i = 0; i < m_BattleActions.Length; i++)
        {
            uniqueActions[i] = Instantiate(m_BattleActions[i]);
        }

        m_BattleActions = uniqueActions;
    }
    
    public void ModifyStat(StatType type, int delta)
    {
        if (StatModifiers.TryGetValue(type, out var modifyAction))
        {
            modifyAction(delta);
            StatChanged?.Invoke(type, delta > 0);
        }
        else
        {
            Utility.LogWarning($"Invalid stat type: {type}");
        }
    }

    public int GetStat(StatType type, TypeModifier modifier = TypeModifier.CURRENT)
    {
        var accessors = modifier == TypeModifier.CURRENT ? CurrentStatAccessors 
                        : modifier == TypeModifier.INITIAL ? InitialStatAccessors 
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
        CurrentMana = CurrentMana < m_BaseMana ? CurrentMana + CurrentManaRegen <= m_BaseMana ? CurrentMana + CurrentManaRegen : m_BaseMana : m_BaseMana;
        CurrentStamina = CurrentStamina < m_BaseStamina ? CurrentStamina + CurrentStaminaRegen <= m_BaseStamina ? CurrentStamina + CurrentStaminaRegen : m_BaseStamina : m_BaseStamina;
    }

    #region Initialization Logic

    private void DetermineScaledValues()
    {
        // ToDo Temporary logic, not sure yet how to handle level scaling, work up a model to finalize
        ScaledHealth = m_BaseHealth * m_ExpLevel;
        ScaledAttack = m_BaseAttack * m_ExpLevel;
        ScaledDefense = m_BaseDefense * m_ExpLevel;
        ScaledSpAttack = m_BaseSpAttack * m_ExpLevel;
        ScaledSpDefense = m_BaseSpDefense * m_ExpLevel;
        ScaledSkill = m_BaseSkill * m_ExpLevel;
    }

    private void InitializeWorkingValues()
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

    private void InitializeModifiersAndAccesors()
    {
        StatModifiers = new Dictionary<StatType, Action<int>>()
        {
            { StatType.HEALTH, value => CurrentHealth = Math.Clamp(CurrentHealth + value, 0, ScaledHealth) },
            { StatType.ATTACK, value => CurrentAttack = Math.Clamp(CurrentAttack + value, 0, ScaledAttack) },
            { StatType.DEFENSE, value => CurrentDefense = Math.Clamp(CurrentDefense + value, 0, ScaledDefense) },
            { StatType.SPATTACK, value => CurrentSpAttack = Math.Clamp(CurrentSpAttack + value, 0, ScaledSpAttack) },
            { StatType.SPDEFENSE, value => CurrentSpDefense = Math.Clamp(CurrentSpDefense + value, 0, ScaledSpDefense) },
            { StatType.SKILL, value => CurrentSkill = Math.Clamp(CurrentSkill + value, 0, ScaledSkill) }
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

        InitialStatAccessors = new Dictionary<StatType, Func<int>>()
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
            // lmao
            {ResourceType.MANA, value => CurrentMana = CurrentMana + value <= m_BaseMana && CurrentMana + value >= 0 ? CurrentMana + value : CurrentMana + value >= m_BaseMana ? m_BaseMana : 0 },
            {ResourceType.STAM, value => CurrentStamina =  CurrentStamina + value <= m_BaseStamina && CurrentStamina + value >= 0 ? CurrentStamina + value : CurrentStamina + value >= m_BaseStamina ? m_BaseStamina : 0 }
        };

        ResourceAccessors = new Dictionary<ResourceType, Func<int>>()
        {
            {ResourceType.MANA, () => CurrentMana },
            {ResourceType.STAM, () => CurrentStamina }
        };
    }
    #endregion
}
