using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Singleton<T> : MonoBehaviour where T : Component
{
    private static T _instance;

    private static bool applicationIsQuitting = false;

    public static T Instance
    {
        get
        {
            if (applicationIsQuitting)
            {
                return null;
            }

            if (_instance == null)
            {
                _instance = FindObjectOfType<T>();


                if (_instance == null)
                {
                    GameObject obj = new()
                    {
                        name = typeof(T).Name
                    };
                    _instance = obj.AddComponent<T>();
                }
            }

            return _instance;
        }
    }

    private void OnDestroy()
    {
        applicationIsQuitting = true;
    }

    public virtual void Awake()
    {
        if (_instance == null)
        {
            _instance = this as T;
            DontDestroyOnLoad(this);
        }
        else if (_instance != this)
        {
            Destroy(gameObject);
        }

        applicationIsQuitting = false;
    }
}
