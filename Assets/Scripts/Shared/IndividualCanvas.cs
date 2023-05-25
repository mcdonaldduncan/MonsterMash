using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class IndividualCanvas : MonoBehaviour
{
    [SerializeField] GameObject m_Health;

    [SerializeField] TextMeshProUGUI m_MonsterName;
    [SerializeField] TextMeshProUGUI m_Attack;
    [SerializeField] TextMeshProUGUI m_Defense;
    [SerializeField] TextMeshProUGUI m_SpAtk;
    [SerializeField] TextMeshProUGUI m_SpDef;
    [SerializeField] TextMeshProUGUI m_Skill;

    Image m_HealthFill;

    private void OnEnable()
    {
        m_HealthFill = m_Health.GetComponent<Image>();
    }

    public void SetDisplayMonster(BattleMonster monster)
    {
        m_MonsterName.text =  monster.Name;
        m_Attack.text = monster.GetStat(StatType.ATTACK, TypeModifier.INITIAL).ToString();
        m_Defense.text = monster.GetStat(StatType.DEFENSE, TypeModifier.INITIAL).ToString();
        m_SpAtk.text = monster.GetStat(StatType.SPATTACK, TypeModifier.INITIAL).ToString();
        m_SpDef.text = monster.GetStat(StatType.SPDEFENSE, TypeModifier.INITIAL).ToString();
        m_Skill.text = monster.GetStat(StatType.SKILL, TypeModifier.INITIAL).ToString();
    }


}
