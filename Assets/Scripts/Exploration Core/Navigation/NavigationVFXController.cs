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
    GameObject PendingVFXInstance;

    Navigator Navigator;

    bool UseBattleIndicator => Navigator?.OnCombatMove ?? false;

    public bool IsActive { get; set; }

    private void Start()
    {
        Navigator = GetComponent<Navigator>();

        PendingVFXInstance = PoolController.Instance.TakeFromPool(PendingVFX, Vector3.zero);

        SetActive(true);
        PrepareTransitions();
    }

    void SetIndicatorLocation(Vector3 location)
    {
        PendingVFXInstance.transform.position = location;
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
            DestinationVFXInstance = PoolController.Instance.TakeFromPool(BattleDestinationVFX, location);
        }
        else
        {
            _ = PoolController.Instance.TakeFromPool(DestinationClickVFX, location);
            DestinationVFXInstance = PoolController.Instance.TakeFromPool(DestinationVFX, location);
        }

        FlipPendingVFX();
        Invoke(nameof(FlipPendingVFX), PendingDisplayDelay);
    }

    void ReturnVFX()
    {
        if (DestinationVFXInstance == null || !DestinationVFXInstance.activeSelf) return;

        PoolController.Instance.ReturnToPool(DestinationVFXInstance);
    }

    void FlipPendingVFX()
    {
        PendingVFXInstance.SetActive(!PendingVFXInstance.activeSelf);
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
        PendingVFXInstance?.SetActive(true);

        Navigator.PathProcessed += SpawnVFX;
        Navigator.StopMove += ReturnVFX;
        Navigator.PathPending += SetIndicatorLocation;
        Navigator.PathMaintain += MaintainBattleIndicator;
    }

    public void Sleep()
    {
        PendingVFXInstance?.SetActive(false);
    }

    public void PrepareTransitions()
    {
        TransitionController.Instance.SubscribeToTransition(GameState.EXPLORATIONACTUAL, Enable);
        TransitionController.Instance.SubscribeToTransition(GameState.BATTLE, Disable);
        TransitionController.Instance.SubscribeToTransition(GameState.HOME, Disable);
    }

    public void DisableTransitions()
    {
        TransitionController.Instance.UnsubscribeFromTransition(GameState.EXPLORATIONACTUAL, Enable);
        TransitionController.Instance.UnsubscribeFromTransition(GameState.BATTLE, Disable);
        TransitionController.Instance.UnsubscribeFromTransition(GameState.HOME, Disable);
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
