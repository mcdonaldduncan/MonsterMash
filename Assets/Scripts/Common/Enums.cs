public enum GameState
{
    HOME,
    HOMEACTUAL,
    EXPLORATION,
    EXPLORATIONACTUAL,
    BATTLE,
    BATTLEACTUAL
}

public enum ElementType
{
    GRASS,
    WATER,
    FIRE,
    SHADOW,
    NEUTRAL
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
    INITIAL,
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

public enum BattleState
{
    PLAYER,
    TRANSITION,
    ENEMY,
    DEFEAT,
    VICTORY
}