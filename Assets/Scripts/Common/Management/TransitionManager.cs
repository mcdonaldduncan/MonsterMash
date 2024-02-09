using System;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class TransitionManager : Singleton<TransitionManager>
{
    GameState m_GameState;

    GameState GameState
    {
        get => m_GameState;

        set
        {
            m_GameState = value;
            HandleTransition();
        }
    }

    public delegate void TransitionDelegate();

    Dictionary<GameState, TransitionDelegate> m_TransitionActions;

    public override void Awake()
    { 
        base.Awake();
        
        CleanTransitions();

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        CleanTransitions();
    }

    void CleanTransitions()
    {
        m_TransitionActions = new Dictionary<GameState, TransitionDelegate>();

        foreach (GameState state in Enum.GetValues(typeof(GameState)))
        {
            m_TransitionActions.Add(state, null);
        }
    }

    public void Transition(GameState newState)
    {
        if (GameState == newState) return;

        GameState = newState;
    }

    public void SubscribeToTransition(GameState gameState, TransitionDelegate method)
    {
        if (m_TransitionActions.ContainsKey(gameState))
        {
            m_TransitionActions[gameState] += method;
        }
        else
        {
            Utility.LogWarning($"No transition action found for game state: {gameState}");
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
            Utility.LogWarning($"No transition action found for game state: {gameState}");
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
            Utility.LogWarning($"Invalid Transition Request");
        }
    }

}


