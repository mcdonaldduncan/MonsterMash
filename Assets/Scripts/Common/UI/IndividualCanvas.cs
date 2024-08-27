using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class IndividualCanvas : MonoBehaviour
{
    [SerializeField] Image m_Health;
    [Header("Health Colors - Place in ascending order")]
    [SerializeField] Color32[] m_Colors;

    CanvasController m_CanvasController;

    StatDisplay[] m_StatDisplays;

    private void Start()
    {
        m_CanvasController = FindObjectOfType<CanvasController>();
        m_StatDisplays = GetComponentsInChildren<StatDisplay>(true);

        foreach (var statDisplay in m_StatDisplays)
        {
            statDisplay.Initialize();
        }

        if (m_CanvasController != null)
        {
            m_CanvasController.SetDisplayMonster += OnSetDisplayMonster;
            m_CanvasController.RefreshHealth += ScaleHealth;
        }

        gameObject.SetActive(false);
    }

    public void OnSetDisplayMonster(BattleMonster monster)
    {
        ScaleHealth(monster);

        foreach (var statDisplay in m_StatDisplays)
        {
            statDisplay.SetText(monster);
        }
    }

    public void ScaleHealth(BattleMonster monster)
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
