using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class AudioController : MonoBehaviour
{
    [SerializeField] AudioSource Exploration;
    [SerializeField] AudioSource Battle;

    [SerializeField] float transitionRate;
    [SerializeField] float volume;

    Coroutine ExploreRoutine;
    Coroutine BattleRoutine;

    private void Start()
    {
        TransitionController.Instance.SubscribeToTransition(GameState.EXPLORATION, OnExploration);
        TransitionController.Instance.SubscribeToTransition(GameState.BATTLE, OnBattle);
    }

    IEnumerator FadeAudio(AudioSource source, float desired)
    {
        while (source.volume != desired)
        {
            source.volume = Mathf.MoveTowards(source.volume, desired, transitionRate * Time.deltaTime);
            yield return null;
        }
    }

    void OnExploration()
    {
        Transition(volume, 0);
    }

    void OnBattle()
    {
        Transition(0, volume);
    }

    void Transition(float explore, float battle)
    {
        if (ExploreRoutine != null) StopCoroutine(ExploreRoutine);
        ExploreRoutine = StartCoroutine(FadeAudio(Exploration, explore));

        if (BattleRoutine != null) StopCoroutine(BattleRoutine);
        BattleRoutine = StartCoroutine(FadeAudio(Battle, battle));
    }
}
