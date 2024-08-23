using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(TextMeshProUGUI))]
public class StatDisplay : MonoBehaviour
{
    [SerializeField] StatType m_Stat;

    TextMeshProUGUI m_Text;

    void Awake()
    {
        m_Text = GetComponent<TextMeshProUGUI>();
    }

    public void SetText(BattleMonster battleMonster)
    {
        m_Text.text = $"{Enum.GetName(typeof(StatType), m_Stat)}: {battleMonster.GetStat(m_Stat)}";
    }
}
