using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManualPoolingVFX : MonoBehaviour, IPoolable
{
    [SerializeField] GameObject m_Prefab;

    public GameObject Prefab { get => m_Prefab; set => m_Prefab = value; }
}
