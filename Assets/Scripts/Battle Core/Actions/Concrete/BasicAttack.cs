using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BasicAttack", menuName = "ScriptableObjects/BasicAttack", order = 1)]
public class BasicAttack : BattleAction
{
    public override void InvokeAction(BattleMonster invoker, BattleMonster target)
    {
        var damageTotal = Power 
            * invoker.GetStat(Modifier)
            * (invoker.Type != Type ? 1f : 1.5f)
            * TypeLookup.GetEfficacy(Type, target.Type); // Not final at all

        // ToDo: Still need to decide how to handle negation from defenses
        
        target.ModifyStat(StatType.HEALTH, -Mathf.RoundToInt(damageTotal));
        Utility.Log($"{invoker.DisplayName} invoked action against {target.DisplayName}, dealing {Mathf.RoundToInt(damageTotal)} damage.");
        Utility.Log($"{invoker.DisplayName} health: {invoker.GetStat(StatType.HEALTH)}");
        Utility.Log($"{target.DisplayName} health: {target.GetStat(StatType.HEALTH)}");
        base.InvokeAction(invoker, target);
    }
}
