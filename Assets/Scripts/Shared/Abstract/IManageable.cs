using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IManageable
{
    bool IsActive { get; set; }

    void SetActive(bool active);

    void Initialize();

    void Sleep();

    void PrepareTransitions();

    void DisableTransitions();

    void Enable();

    void Disable();
}
