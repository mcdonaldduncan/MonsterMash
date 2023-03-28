using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleMonster : MonoBehaviour
{
    [Header("Monster Level")]
    [Range(1, 100)]
    [SerializeField] int m_ExpLevel;

    [Header("Base Stats")]
    [SerializeField] float m_BaseHealth;
    [SerializeField] float m_BaseAttack;
    [SerializeField] float m_BaseDefense;
    [SerializeField] float m_BaseSpAttack;
    [SerializeField] float m_BaseSpDefense;
    [SerializeField] float m_BaseMana;
    [SerializeField] float m_BaseStamina;


    [Header("Move Set")]
    [SerializeField] BattleAction[] m_BattleActions;

    //[Header("Current Stats")]
    //public readonly float m_CurrentHealth => m_Health;

    float m_Health;

    
}
