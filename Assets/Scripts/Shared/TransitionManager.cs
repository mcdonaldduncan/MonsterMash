using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransitionManager : Singleton<TransitionManager>
{
    [SerializeField] GameState m_GameState;

    public delegate void TransitionDelegate();

    Dictionary<GameState, TransitionDelegate> m_TransitionActions;

    public override void Awake()
    { 
        base.Awake();

        m_TransitionActions = new Dictionary<GameState, TransitionDelegate>();

        foreach (GameState state in Enum.GetValues(typeof(GameState)))
        {
            m_TransitionActions.Add(state, null);
        }
    }

    public void Transition(GameState newState)
    {
        if (m_GameState == newState) return;

        m_GameState = newState;
        HandleTransition();
    }

    public void SubscribeToTransition(GameState gameState, TransitionDelegate method)
    {
        if (m_TransitionActions.ContainsKey(gameState))
        {
            m_TransitionActions[gameState] += method;
        }
        else
        {
            Debug.LogWarning($"No transition action found for game state: {gameState}");
        }
    }

    public void UnsubscribeFromTransition(GameState gameState, TransitionDelegate method)
    {
        if (m_TransitionActions.ContainsKey(gameState))
        {
            m_TransitionActions[gameState] -= method;
        }
        else
        {
            Debug.LogWarning($"No transition action found for game state: {gameState}");
        }
    }

    private void HandleTransition()
    {
        if (m_TransitionActions.TryGetValue(m_GameState, out TransitionDelegate transition))
        {
            transition?.Invoke();
        }
        else
        {
            Debug.LogWarning($"Invalid Transition Request");
        }
    }

}

public enum GameState
{
    EXPLORATION,
    BATTLE,
    HOME
}
