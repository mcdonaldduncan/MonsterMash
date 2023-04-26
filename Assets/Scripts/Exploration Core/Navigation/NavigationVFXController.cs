using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Navigator))]
public class NavigationVFXController : MonoBehaviour
{
    [SerializeField] GameObject DestinationVFX;
    [SerializeField] GameObject DestinationClickVFX;
    [SerializeField] GameObject PendingVFX;
    [SerializeField] GameObject BattleDestinationVFX;

    [SerializeField] float PendingDisplayDelay;

    GameObject DestinationVFXInstance;

    Navigator Navigator;

    bool UseBattleIndicator => Navigator?.OnCombatMove ?? false;

    private void Start()
    {
        Navigator = GetComponent<Navigator>();

        Navigator.PathProcessed += SpawnVFX;
        Navigator.StopMove += ReturnVFX;
        Navigator.PathPending += SetIndicatorLocation;
    }

    void SetIndicatorLocation(Vector3 location)
    {
        PendingVFX.transform.position = location;
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

}
