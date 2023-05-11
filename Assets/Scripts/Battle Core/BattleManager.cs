using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BattleManager : Singleton<BattleManager>
{
    public BattleMonster Player;
    public BattleMonster Enemy;

    Vector3 m_CameraOffset;

    float min = 3f;
    float max = 4f;

    float sampleDistance = 0;

    private void Start()
    {
        TypeLookup.Init();

        TransitionManager.Instance.SubscribeToTransition(GameState.BATTLE, OnBattle);
        TransitionManager.Instance.SubscribeToTransition(GameState.BATTLEACTUAL, OnBattleActual);

        TransitionManager.Instance.SubscribeToTransition(GameState.EXPLORATIONACTUAL, OnExplorationActual);
    }

    public void Init(BattleMonster player, BattleMonster enemy)
    {
        Player = player;
        Enemy = enemy;
    }

    Vector3 CalculateEnemyPos()
    {
        NavMeshHit hit;
        var newPoint = Player.transform.position + new Vector3(Random.Range(min, max), 0, Random.Range(min, max));
        
        if (!NavMesh.SamplePosition(newPoint, out hit, 1f + sampleDistance, NavMesh.AllAreas))
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

        //CameraManager.Instance.SetTargets(m_Player.transform, m_Enemy.transform);
    }

    void OnBattleActual()
    {
        if (Enemy == null) return;

        Enemy.GetComponent<AINavigationController>().SetPosition(CalculateEnemyPos());

        Player.transform.LookAt(Enemy.transform);
        Enemy.transform.LookAt(Player.transform);

        //m_CameraOffset = m_Player.transform.up + m_Player.transform.right - m_Player.transform.forward;

        //CameraManager.Instance.SetPosition(m_Player.transform.position + m_CameraOffset.normalized * 2f);
        //Utility.Log(Vector3.Distance(m_Player.transform.position, Camera.main.transform.position).ToString());
    }

    void OnExplorationActual()
    {
        if (Enemy == null) return;
        Enemy.GetComponent<AINavigationController>().SetActive(true);
    }
}
