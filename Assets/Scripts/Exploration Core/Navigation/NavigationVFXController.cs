using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(Navigator))]
public class NavigationVFXController : MonoBehaviour, IManageable
{
    [SerializeField] GameObject DestinationVFX;
    [SerializeField] GameObject DestinationClickVFX;
    [SerializeField] GameObject PendingVFX;
    [SerializeField] GameObject BattleDestinationVFX;

    [SerializeField] float PendingDisplayDelay;

    GameObject DestinationVFXInstance;

    Navigator Navigator;

    bool UseBattleIndicator => Navigator?.OnCombatMove ?? false;

    public bool IsActive { get; set; }

    private void Start()
    {
        Navigator = GetComponent<Navigator>();

        SetActive(true);
        PrepareTransitions();
    }

    void SetIndicatorLocation(Vector3 location)
    {
        PendingVFX.transform.position = location;
    }

    void MaintainBattleIndicator(Vector3 location)
    {
        if (UseBattleIndicator && DestinationVFXInstance != null) DestinationVFXInstance.transform.position = location;
    }

    void SpawnVFX(Vector3 location)
    {
        ReturnVFX();

        if (UseBattleIndicator)
        {
            DestinationVFXInstance = PoolManager.Instance.TakeFromPool(BattleDestinationVFX, location);
        }
        else
        {
            _ = PoolManager.Instance.TakeFromPool(DestinationClickVFX, location);
            DestinationVFXInstance = PoolManager.Instance.TakeFromPool(DestinationVFX, location);
        }

        FlipPendingVFX();
        Invoke(nameof(FlipPendingVFX), PendingDisplayDelay);
    }

    void ReturnVFX()
    {
        if (DestinationVFXInstance == null || !DestinationVFXInstance.activeSelf) return;

        PoolManager.Instance.ReturnToPool(DestinationVFXInstance);
    }

    void FlipPendingVFX()
    {
        PendingVFX.SetActive(!PendingVFX.activeSelf);
    }

    public void SetActive(bool active)
    {
        if (IsActive == active) return;

        IsActive = active;

        if (IsActive) Wake();
        else Sleep();
    }

    public void Wake()
    {
        PendingVFX?.SetActive(true);

        Navigator.PathProcessed += SpawnVFX;
        Navigator.StopMove += ReturnVFX;
        Navigator.PathPending += SetIndicatorLocation;
        Navigator.PathMaintain += MaintainBattleIndicator;
    }

    public void Sleep()
    {
        PendingVFX?.SetActive(false);
    }

    public void PrepareTransitions()
    {
        TransitionManager.Instance.SubscribeToTransition(GameState.EXPLORATIONACTUAL, Enable);
        TransitionManager.Instance.SubscribeToTransition(GameState.BATTLE, Disable);
        TransitionManager.Instance.SubscribeToTransition(GameState.HOME, Disable);
    }

    public void DisableTransitions()
    {
        TransitionManager.Instance.UnsubscribeFromTransition(GameState.EXPLORATIONACTUAL, Enable);
        TransitionManager.Instance.UnsubscribeFromTransition(GameState.BATTLE, Disable);
        TransitionManager.Instance.UnsubscribeFromTransition(GameState.HOME, Disable);
    }

    public void Enable()
    {
        SetActive(true);
    }

    public void Disable()
    {
        SetActive(false);
    }
}
