using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BattleController : Singleton<BattleController>
{
    public BattleMonster Player;
    public BattleMonster Enemy;

    readonly float min = 3f;
    readonly float max = 4f;

    float sampleDistance = 0;

    private void Start()
    {
        TypeLookup.Init();

        TransitionController.Instance.SubscribeToTransition(GameState.BATTLE, OnBattle);
        TransitionController.Instance.SubscribeToTransition(GameState.BATTLEACTUAL, OnBattleActual);

        TransitionController.Instance.SubscribeToTransition(GameState.EXPLORATIONACTUAL, OnExplorationActual);
    }

    public void Init(BattleMonster player, BattleMonster enemy)
    {
        Player = player;
        Enemy = enemy;
    }

    Vector3 CalculateEnemyPos()
    {
        var newPoint = Player.transform.position + new Vector3(Random.Range(min, max), 0, Random.Range(min, max));

        if (!NavMesh.SamplePosition(newPoint, out NavMeshHit hit, 1f + sampleDistance, NavMesh.AllAreas))
        {
            sampleDistance++;
            return CalculateEnemyPos();
        }

        sampleDistance = 0;
        return hit.position;
    }

    void OnBattle()
    {
        if (Enemy == null) return;

        Enemy.GetComponent<AINavigationController>().SetActive(false);
    }

    void OnBattleActual()
    {
        if (Enemy == null) return;

        Enemy.GetComponent<AINavigationController>().SetPosition(CalculateEnemyPos());

        Player.transform.LookAt(Enemy.transform);
        Enemy.transform.LookAt(Player.transform);
    }

    void OnExplorationActual()
    {
        if (Enemy == null) return;
        Enemy.GetComponent<AINavigationController>().SetActive(true);
    }
}
