using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleAction : MonoBehaviour, IAction
{
    [SerializeField] string m_Name;
    [SerializeField] string m_Cost;

    public string Name { get; set; }
    public int Cost { get; set; }

    public void Invoke(BattleMonster invoker, BattleMonster target)
    {
        
    }

    public virtual void UseEffect()
    {

    }
}
