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

        // ToDo: Probably dont need to hold a ref to the particleSystem, component is required so just run it all in one line?
        if (m_ParticleSystem == null) // invert?
        {
            m_ParticleSystem = GetComponent<ParticleSystem>();
        }

        var mainModule = m_ParticleSystem.main;
        mainModule.stopAction = ParticleSystemStopAction.Disable;

        IsInitialized = true;
    }

    private void OnDisable()
    {
        if (PoolController.Instance == null || gameObject == null) return;
        PoolController.Instance.ReturnToPool(gameObject);
    }
}
