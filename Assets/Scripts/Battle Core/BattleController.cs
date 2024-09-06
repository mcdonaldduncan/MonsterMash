using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public class BattleController : Singleton<BattleController>
{
    [SerializeField] float m_TransitionTime;

    [NonSerialized] public BattleMonster Player;
    [NonSerialized] public BattleMonster Enemy;

    readonly float min = 3f;
    readonly float max = 4f;

    float m_SampleDistance = 0;
    
    BattleState m_State;

    WaitForSeconds m_TransitionWFS;

    AiDecisionController m_DecisionController;

    BattleAction m_DefaultAction;

    private void Start()
    {
        TypeLookup.Init();

        TransitionController.Instance.SubscribeToTransition(GameState.BATTLE, OnBattle);
        TransitionController.Instance.SubscribeToTransition(GameState.BATTLEACTUAL, OnBattleActual);
        TransitionController.Instance.SubscribeToTransition(GameState.EXPLORATIONACTUAL, OnExplorationActual);

        m_TransitionWFS = new WaitForSeconds(m_TransitionTime);

        m_DecisionController = new AiDecisionController();
    }

    public void Init(BattleMonster player, BattleMonster enemy)
    {
        Player = player;
        Enemy = enemy;
    }

    private Vector3 CalculateEnemyPos()
    {
        var newPoint = Player.transform.position + new Vector3(Random.Range(min, max), 0, Random.Range(min, max));

        if (!NavMesh.SamplePosition(newPoint, out NavMeshHit hit, 1f + m_SampleDistance, NavMesh.AllAreas))
        {
            m_SampleDistance++;
            return CalculateEnemyPos();
        }

        m_SampleDistance = 0;
        return hit.position;
    }

    private void OnBattle()
    {
        if (Enemy == null) return;

        Enemy.GetComponent<AINavigationController>().SetActive(false); // we only turn off the navigation for the current enemy
    }

    private void OnBattleActual()
    {
        if (Enemy == null) return;

        Enemy.GetComponent<AINavigationController>().SetPosition(CalculateEnemyPos());

        Player.transform.LookAt(Enemy.transform);
        Enemy.transform.LookAt(Player.transform);

        SetState(Player.GetStat(StatType.SKILL) >= Enemy.GetStat(StatType.SKILL) ? BattleState.PLAYER : BattleState.TRANSITION);
    }

    private void OnExplorationActual()
    {
        if (Enemy == null || Enemy.GetStat(StatType.HEALTH) <= 0) return;
        Enemy.GetComponent<AINavigationController>().SetActive(true);
    }

    public void NotifyController(BattleMonster monster)
    {
        if (monster == Player && m_State != BattleState.PLAYER) return;
        if (monster == Enemy && m_State != BattleState.ENEMY) return;

        if (Player.GetStat(StatType.HEALTH) <= 0 || Enemy.GetStat(StatType.HEALTH) <= 0) // all enemies for eventual doubles/more
        {
            EndBattle();
            return;
        }

        SetState(BattleState.TRANSITION);
    }

    private IEnumerator TransitionRoutine(BattleState targetState)
    {
        yield return m_TransitionWFS;

        SetState(targetState);
    }

    private void SetState(BattleState value)
    {
        if (value == BattleState.TRANSITION)
        {
            StartCoroutine(TransitionRoutine(m_State == BattleState.PLAYER ? BattleState.ENEMY : BattleState.PLAYER));
        }

        m_State = value;

        CanvasController.Instance.SetPlayerActionState(m_State == BattleState.PLAYER); // need to make this disable flee as well
        if (m_State == BattleState.ENEMY) SimulateEnemyTurn();
    }

    private BattleAction BestMove(BattleMonster monster)
    {
        // Note for future me: Assign all available actions a probability based on efficacy + other considerations (low health heal, status, stat boosts, etc)
        if (m_DefaultAction == null)
        {
            m_DefaultAction = ScriptableObject.CreateInstance<BasicAttack>();
        }

        if (monster.Actions == null || monster.Actions.Length == 0) return m_DefaultAction;

        if (monster.Actions.Length == 1) return monster.Actions[0];


        if (monster.GetStat(StatType.HEALTH) > 0)
        {

        }

        return monster.Actions[0];
    }

    private void SimulateEnemyTurn()
    {
        var action = BestMove(Enemy);

        if (action == null) return;

        action.InvokeAction(Enemy, Player);
    }

    private void EndBattle()
    {
        if (Player.GetStat(StatType.HEALTH) > 0) Victory();
        else Defeat();

        TransitionController.Instance.Transition(GameState.EXPLORATION); // this should happen after all anims/text plays out in the future
    }

    private void Victory()
    {
        // Apply xp
        // Animations / text
        TransitionController.Instance.SubscribeToTransition(GameState.EXPLORATIONACTUAL, RemoveEnemy);
    }

    private void Defeat()
    {
        // animations / text
        // Send to? menu / new monster / home

    }

    private void RemoveEnemy() //s
    {
        Enemy.gameObject.SetActive(false);
        TransitionController.Instance.UnsubscribeFromTransition(GameState.EXPLORATIONACTUAL, RemoveEnemy);
    }

    // Bug: upon defeat, transtion starts and completes correctly but enemy simulates turn
    // Repro: second battle after red, vs green snek - not tested for consistency
}
