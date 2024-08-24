using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class IndividualCanvas : MonoBehaviour
{
    [SerializeField] GameObject m_Health;

    StatDisplay[] m_StatDisplays;

    Image m_HealthFill;

    private void Start()
    {
        m_HealthFill = m_Health.GetComponent<Image>();
        m_StatDisplays = GetComponentsInChildren<StatDisplay>(true);

        foreach (var statDisplay in m_StatDisplays)
        {
            statDisplay.Initialize();
        }

        gameObject.SetActive(false);
    }

    public void SetDisplayMonster(BattleMonster monster)
    {
        foreach (var statDisplay in m_StatDisplays)
        {
            statDisplay.SetText(monster);
        }
    }
}
