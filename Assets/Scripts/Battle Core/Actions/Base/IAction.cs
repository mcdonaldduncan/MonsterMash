using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IAction
{
    public string Name { get; }
    public int Cost { get; }
    public int Power { get; }

    public StatType Modifier { get; }

    public ElementType Type { get; }

    public void Invoke(BattleMonster invoker, BattleMonster target);

}