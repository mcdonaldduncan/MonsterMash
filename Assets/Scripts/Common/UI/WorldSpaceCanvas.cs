using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldSpaceCanvas : MonoBehaviour
{
    Transform m_Target;
    Transform m_Transform;

    private void OnEnable()
    {
        m_Target = Camera.main.transform;
        m_Transform = transform;
    }

    private void LateUpdate()
    {
        m_Transform.LookAt(m_Target);
        m_Transform.Rotate(0, 180, 0);
    }
}
