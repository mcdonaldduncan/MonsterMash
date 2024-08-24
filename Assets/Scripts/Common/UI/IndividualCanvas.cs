using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class IndividualCanvas : MonoBehaviour
{
    [SerializeField] GameObject m_Health;

    CanvasController m_CanvasController;

    StatDisplay[] m_StatDisplays;

    Image m_HealthFill;

    private void Start()
    {
        m_CanvasController = FindObjectOfType<CanvasController>();
        m_HealthFill = m_Health.GetComponent<Image>();
        m_StatDisplays = GetComponentsInChildren<StatDisplay>(true);

        foreach (var statDisplay in m_StatDisplays)
        {
            statDisplay.Initialize();
        }

        if (m_CanvasController != null)
        {
            m_CanvasController.SetDisplayMonster += OnSetDisplayMonster;
        }

        gameObject.SetActive(false);
    }

    public void OnSetDisplayMonster(BattleMonster monster)
    {
        foreach (var statDisplay in m_StatDisplays)
        {
            statDisplay.SetText(monster);
        }
    }

    // ToDo repurpose for healthbar, from my game potionPanic
    //public void ScaleHealth()
    //{
    //    float healthBarZ = (float)Health / (float)startingHealth;

    //    healthBar.transform.localScale = new Vector3(healthScale.x, healthScale.y, healthBarZ);

    //    if (Health <= (float)startingHealth / 3f)
    //    {
    //        healthRend.material = lowHealth;
    //    }
    //    else if (Health <= (float)startingHealth / 3f * 2f)
    //    {
    //        healthRend.material = medHealth;
    //    }
    //    else
    //    {
    //        healthRend.material = highHealth;
    //    }
    //}
}
