using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BasicAttack", menuName = "ScriptableObjects/BasicAttack", order = 1)]
public class BasicAttack : BattleAction
{
    public override void InvokeAction(BattleMonster invoker, BattleMonster target)
    {
        var damageTotal = Power * invoker.GetStat(Modifier);
        damageTotal = Mathf.RoundToInt(damageTotal * TypeLookup.GetEfficacy(Type, target.Type));
        target.ModifyStat(StatType.HEALTH, -damageTotal);
    }
}
