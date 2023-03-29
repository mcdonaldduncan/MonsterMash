using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IAction
{
    public string Name { get; set; }
    public int Cost { get; set; }

    public ActionModifier Modifier { get; set; }

    public ActionType Type { get; set; }

    public void Invoke(BattleMonster invoker, BattleMonster target);

}


public enum ActionModifier
{
    ATK,
    SPATK,
    SKL
}

public enum ActionType
{
    GRASS,
    WATER,
    FIRE,
    SHADOW
}