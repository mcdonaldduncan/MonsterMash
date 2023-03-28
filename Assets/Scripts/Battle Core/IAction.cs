using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IAction
{
    public string Name { get; set; }
    public int Cost { get; set; }

    public void Invoke(BattleMonster invoker, BattleMonster target);

}
