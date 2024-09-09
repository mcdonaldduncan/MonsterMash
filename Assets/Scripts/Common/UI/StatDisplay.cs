using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class StatDisplay : TextDisplay
{
    [SerializeField] StatType Stat;

    public void SetText(BattleMonster battleMonster)
    {
        SetText($"{Enum.GetName(typeof(StatType), Stat)}: {battleMonster.GetStat(Stat)}");
    }
}
