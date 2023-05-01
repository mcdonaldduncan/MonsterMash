using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicAttack : BattleAction
{
    public override void Invoke(BattleMonster invoker, BattleMonster target)
    {
        int damageTotal = Power * invoker.GetStat(StatType.ATTACK);

        damageTotal = Mathf.RoundToInt(damageTotal * TypeLookup.GetEfficacy(invoker.Type, target.Type));

        target.ModifyStat(StatType.HEALTH, -damageTotal);
    }
}
