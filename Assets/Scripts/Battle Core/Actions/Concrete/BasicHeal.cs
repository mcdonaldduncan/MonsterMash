using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicHeal : BattleAction
{
    public override void Invoke(BattleMonster invoker, BattleMonster target)
    {
        int healTotal = Power * invoker.GetStat(StatType.SKILL);
        target.AlterStat(StatType.HEALTH, healTotal);
    }
}
