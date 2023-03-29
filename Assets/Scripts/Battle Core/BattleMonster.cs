using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    int m_CurrentHealth;
    int m_CurrentAttack;
    int m_CurrentDefense;
    int m_CurrentSpAttack;
    int m_CurrentSpDefense;
    int m_CurrentSkill;

    // Working values for resource pools
    int m_CurrentMana;
    int m_CurrentStamina;
    

    void DetermineScaledValues()
    {
        m_ScaledHealth = 0;
        m_ScaledAttack = 0;
        m_ScaledDefense = 0;
        m_ScaledSpAttack = 0;
        m_ScaledSpDefense = 0;
        m_ScaledSkill = 0;
    }

    void InitializeWorkingValues()
    {
        m_CurrentHealth = m_ScaledHealth;
        m_CurrentAttack = m_ScaledAttack;
        m_CurrentDefense = m_ScaledDefense;
        m_CurrentSpAttack = m_ScaledSpAttack;
        m_CurrentSpDefense = m_ScaledSpDefense;
        m_CurrentSkill = m_ScaledSkill;

        m_CurrentMana = m_BaseMana;
        m_CurrentStamina = m_BaseStamina;
    }
}
