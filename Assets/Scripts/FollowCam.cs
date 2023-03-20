using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCam : MonoBehaviour
{
    [SerializeField] Transform Target;

    Vector3 TargetOffset;

    void Start()
    {
        TargetOffset = transform.position - Target.position;
    }

    void Update()
    {
        transform.position = Target.position + TargetOffset;
    }
}
