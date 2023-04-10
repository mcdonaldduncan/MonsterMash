using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BattleAction : MonoBehaviour, IAction
{
    [SerializeField] string m_Name;
    [SerializeField] string m_Cost;
    [SerializeField] ActionModifier m_ActionModifier;
    [SerializeField] ActionType m_ActionType;

    public string Name { get; set; }
    public int Cost { get; set; }
    public int Power { get; set; }
    public ActionModifier Modifier { get; set; }
    public ElementType Type { get; set; }
    

    public virtual void Invoke(BattleMonster invoker, BattleMonster target) { }

}
