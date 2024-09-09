using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class CanvasController : Singleton<CanvasController>
{
    [SerializeField] GameObject m_ActionPanel;
    [SerializeField] GameObject m_RunButton;
    [SerializeField] GameObject m_IndividualCanvas;

    [SerializeField] Image m_FadeImage;

    [SerializeField] float m_FadeRate;
    [SerializeField] float m_CutDuration;
    [SerializeField] float m_ICHeightOffset;
    [SerializeField] float m_ICTimerDuration;

    BattleMonster m_CurrentMonster;

    Coroutine m_FadeCoroutine;
    Coroutine m_ICTimerCoroutine;

    Dictionary<Collider, BattleMonster> m_MonsterLookup;

    Button[] m_ActionButtons;
    readonly StatDisplay[] m_StatDisplays;

    float m_FadeAlpha;
    float m_ICTimerStart;

    public delegate void IndividualCanvasControlDelegate(BattleMonster monster);
    public event IndividualCanvasControlDelegate SetDisplayMonster;
    public event IndividualCanvasControlDelegate RefreshDisplay;

    bool ShouldICDisable => Time.time > m_ICTimerStart + m_ICTimerDuration;

    private void OnEnable()
    {
        m_ActionButtons = m_ActionPanel.GetComponentsInChildren<Button>(true);

        m_ActionPanel.SetActive(false);
        m_RunButton.SetActive(false);

        SetupMonsterLookup();

        m_IndividualCanvas.transform.localPosition = Vector3.up * m_ICHeightOffset;
    }

    private void Start()
    {
        TransitionController.Instance.SubscribeToTransition(GameState.BATTLE, OnBattle);
        TransitionController.Instance.SubscribeToTransition(GameState.BATTLEACTUAL, OnBattleActual);
        TransitionController.Instance.SubscribeToTransition(GameState.EXPLORATION, OnExploration);
        TransitionController.Instance.SubscribeToTransition(GameState.EXPLORATIONACTUAL, OnExplorationActual);
    }

    private void SetupButtons(BattleMonster player, BattleMonster target)
    {
        var actions = player.Actions;
        for (var i = 0; i < m_ActionButtons.Length; i++)
        {
            if (i >= actions.Length)
            {
                m_ActionButtons[i].interactable = false;
                m_ActionButtons[i].GetComponentInChildren<TextMeshProUGUI>().text = string.Empty;
                continue;
            }

            var actionIndex = i;

            m_ActionButtons[i].GetComponentInChildren<TextMeshProUGUI>().text = actions[i].Name; // todo: set these up with a script to hold text ref, starting to getcomponent too much
            m_ActionButtons[i].onClick.RemoveAllListeners();
            m_ActionButtons[i].onClick.AddListener(delegate { actions[actionIndex].InvokeAction(player, target); });
        }
    }

    public void SetPlayerActionState(bool state)
    {
        foreach (var button in m_ActionButtons)
        {
            button.interactable = state;
        }
    }

    private void OnBattle()
    {
        if (m_FadeCoroutine != null) StopCoroutine(m_FadeCoroutine);
        m_FadeCoroutine = StartCoroutine(RunFadeTransition(GameState.BATTLEACTUAL));
    }

    private void OnExploration()
    {
        if (m_FadeCoroutine != null) StopCoroutine(m_FadeCoroutine);
        m_FadeCoroutine = StartCoroutine(RunFadeTransition(GameState.EXPLORATIONACTUAL));
    }

    private void OnBattleActual()
    {
        m_ActionPanel.SetActive(true);
        m_RunButton.SetActive(true);

        SetupButtons(BattleController.Instance.Player, BattleController.Instance.Enemy);

        Invoke(nameof(InvokeableFade), m_CutDuration);
    }

    private void OnExplorationActual()
    {
        m_ActionPanel.SetActive(false);
        m_RunButton.SetActive(false);

        Invoke(nameof(InvokeableFade), m_CutDuration);
    }

    private void InvokeableFade()
    {
        if (m_FadeCoroutine != null) StopCoroutine(m_FadeCoroutine);
        m_FadeCoroutine = StartCoroutine(RunDeFadeTransition());
    }

    private IEnumerator RunFadeTransition(GameState state)
    {
        while (m_FadeAlpha < 1f)
        {
            m_FadeAlpha = Mathf.MoveTowards(m_FadeAlpha, 1, m_FadeRate * Time.deltaTime);
            var temp = m_FadeImage.color;
            temp.a = m_FadeAlpha;
            m_FadeImage.color = temp;

            yield return null;
        }

        TransitionController.Instance.Transition(state);
    }

    private IEnumerator RunDeFadeTransition()
    {
        while (m_FadeAlpha > 0f)
        {
            m_FadeAlpha = Mathf.MoveTowards(m_FadeAlpha, 0, m_FadeRate * Time.deltaTime);
            var temp = m_FadeImage.color;
            temp.a = m_FadeAlpha;
            m_FadeImage.color = temp;

            yield return null;
        }
    }

    private void SetupMonsterLookup()
    {
        m_MonsterLookup = new Dictionary<Collider, BattleMonster>();

        var monsters = FindObjectsOfType<BattleMonster>();

        foreach (var monster in monsters)
        {
            if (!monster.TryGetComponent<Collider>(out var col))
            {
                Utility.LogError("Collider not found with BattleMonster");
                continue;
            }

            m_MonsterLookup.Add(col, monster);
        }
    }

    public void ExitBattle()
    {
        TransitionController.Instance.Transition(GameState.EXPLORATION);
    }

    public void RefreshICTimer(Collider col)
    {
        if (!m_MonsterLookup.TryGetValue(col, out var battleMonster)) return;

        if (m_CurrentMonster == null || battleMonster.Id != m_CurrentMonster.Id)
        {
            //ToDo: Add a sprite to display type

            m_CurrentMonster = battleMonster;
            m_IndividualCanvas.transform.SetParent(m_CurrentMonster.transform, false);
            
            SetDisplayMonster?.Invoke(m_CurrentMonster);
        }

        RefreshDisplay?.Invoke(m_CurrentMonster); // come up with something better than this, kinda defeats the purpose of above efficiency

        m_ICTimerStart = Time.time;

        m_ICTimerCoroutine ??= StartCoroutine(WatchIndividualCanvasTimer());
        
        if (!m_IndividualCanvas.activeSelf) m_IndividualCanvas.SetActive(true);
    }

    // Might be a performance hit on gc
    private IEnumerator WatchIndividualCanvasTimer()
    {
        while (!ShouldICDisable)
        {
            yield return null;
        }

        m_IndividualCanvas.SetActive(false);
        m_ICTimerCoroutine = null;
    }
}
