using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicHeal : BattleAction
{
    public override void InvokeAction(BattleMonster invoker, BattleMonster target)
    {
        int healTotal = Power * invoker.GetStat(StatType.SKILL);
        target.ModifyStat(StatType.HEALTH, healTotal);
    }
}
