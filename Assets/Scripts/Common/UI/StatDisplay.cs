using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(TextMeshProUGUI))]
public class StatDisplay : MonoBehaviour
{
    [SerializeField] StatType Stat;

    TextMeshProUGUI m_Text;

    public void Initialize()
    {
        if (m_Text != null) return;
        m_Text = GetComponent<TextMeshProUGUI>();
    }

    public void SetText(BattleMonster battleMonster)
    {
        if (m_Text == null)
        {
            m_Text = GetComponent<TextMeshProUGUI>();
        }

        m_Text.text = $"{Enum.GetName(typeof(StatType), Stat)}: {battleMonster.GetStat(Stat)}";
    }
}
