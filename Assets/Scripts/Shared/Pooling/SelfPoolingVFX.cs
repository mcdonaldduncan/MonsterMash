using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
public class SelfPoolingVFX : MonoBehaviour, IPoolable
{
    [SerializeField] GameObject m_Prefab;

    ParticleSystem m_ParticleSystem;

    bool IsInitialized;

    public GameObject Prefab { get => m_Prefab; set => m_Prefab = value; }

    private void OnEnable()
    {
        if (IsInitialized) return;

        if (m_ParticleSystem == null)
        {
            m_ParticleSystem = GetComponent<ParticleSystem>();
        }

        var mainModule = m_ParticleSystem.main;
        mainModule.stopAction = ParticleSystemStopAction.Disable;

        IsInitialized = true;
    }

    private void OnDisable()
    {
        if (PoolManager.Instance == null || gameObject == null) return;
        PoolManager.Instance.ReturnToPool(gameObject);
    }
}
