using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameState
{
    EXPLORATION,
    BATTLE,
    HOME
}

public enum ElementType
{
    GRASS,
    WATER,
    FIRE,
    SHADOW
}

public enum StatType
{
    HEALTH,
    ATTACK,
    DEFENSE,
    SPATTACK,
    SPDEFENSE,
    SKILL
}

public enum TypeModifier
{
    CURRENT,
    MAX,
    BASE
}

public enum ResourceType
{
    MANA,
    STAM
}

public enum AgentState
{
    WANDER,
    CHASE,
    IDLE,
    SEARCH,
    FLEE

}