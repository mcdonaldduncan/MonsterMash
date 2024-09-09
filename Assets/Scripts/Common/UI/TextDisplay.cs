
using System;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(TextMeshProUGUI))]
public abstract class TextDisplay : MonoBehaviour
{
    protected TextMeshProUGUI m_Text;

    public void Initialize()
    {
        if (m_Text != null) return;
        m_Text = GetComponent<TextMeshProUGUI>();
    }

    public virtual void SetText(string text)
    {
        if (m_Text == null)
        {
            m_Text = GetComponent<TextMeshProUGUI>();
        }

        m_Text.text = $"{text}";
    }
}