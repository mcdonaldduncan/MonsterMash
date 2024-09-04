using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class IndividualCanvas : MonoBehaviour
{
    [SerializeField] Image m_Health;
    [SerializeField] TextMeshProUGUI m_NameText;
    [Header("Health Colors - Place in ascending order")]
    [SerializeField] Color32[] m_Colors;

    StatDisplay[] m_StatDisplays;

    private void Start()
    {
        m_StatDisplays = GetComponentsInChildren<StatDisplay>(true);

        foreach (var statDisplay in m_StatDisplays)
        {
            statDisplay.Initialize();
        }

        CanvasController.Instance.SetDisplayMonster += OnSetDisplayMonster;
        CanvasController.Instance.RefreshDisplay += OnRefreshDisplay;

        gameObject.SetActive(false);
    }

    public void OnSetDisplayMonster(BattleMonster monster)
    {
        m_NameText.text = monster.DisplayName;

        foreach (var statDisplay in m_StatDisplays)
        {
            statDisplay.SetText(monster);
        }
    }

    public void OnRefreshDisplay(BattleMonster monster)
    {
        ScaleHealth(monster);

        m_NameText.transform.parent.gameObject.SetActive(!(TransitionController.Instance.GetState() == GameState.BATTLEACTUAL));

        foreach (var statDisplay in m_StatDisplays)
        {
            statDisplay.gameObject.SetActive(!(TransitionController.Instance.GetState() == GameState.BATTLEACTUAL));
        }
    }

    private void ScaleHealth(BattleMonster monster)
    {
        float currentHealth = monster.GetStat(StatType.HEALTH);
        float initialHealth = monster.GetStat(StatType.HEALTH, TypeModifier.INITIAL);

        var healthScale = currentHealth / initialHealth;

        m_Health.rectTransform.localScale = new Vector3(healthScale, 1, 1);

        if (m_Colors.Length <= 1) return;

        var relativeHealth = currentHealth / (initialHealth / m_Colors.Length);

        m_Health.color = m_Colors[Math.Max(Mathf.FloorToInt(relativeHealth) - 1, 0)];
    }
}
