using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TypewriterDisplay : TextDisplay
{
    [SerializeField] float m_CharacterDelay;

    string m_Desired;
    string m_Current;

    int m_Index;

    float m_LastCharacterTime;

    bool ShouldIncrementWriter => Time.time > m_LastCharacterTime + m_CharacterDelay && m_Index < m_Desired.Length;

    void Update()
    {
        if (m_Current == m_Desired) return;

        if (!ShouldIncrementWriter) return;

        m_Current = m_Desired[..++m_Index];
        m_Current += m_Index < m_Desired.Length ? "_" : string.Empty;
        m_LastCharacterTime = Time.time;

        base.SetText(m_Current);
    }

    public override void SetText(string text)
    {
        if (m_Current == text || m_Desired == text) return;

        m_Index = 0;
        m_Desired = text;

        if (text == string.Empty)
        {
            m_Current = text;
            base.SetText(text);
        }
    }
}
