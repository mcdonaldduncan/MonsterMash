using System.Collections.Generic;
using UnityEngine;

public class PoolController : Singleton<PoolController>
{
    Dictionary<GameObject, Queue<GameObject>> m_Pool;

    private void OnEnable()
    {
        m_Pool = new Dictionary<GameObject, Queue<GameObject>>();
    }

    public GameObject TakeFromPool(GameObject prefab, Vector3 startLocation)
    {
        if (!m_Pool.ContainsKey(prefab))
        {
            m_Pool.Add(prefab, new Queue<GameObject>());
        }

        if (m_Pool[prefab].Count > 0)
        {
            GameObject obj = m_Pool[prefab].Dequeue();
            if (obj == null) return TakeFromPool(prefab, startLocation);
            obj.transform.position = startLocation;
            obj.SetActive(true);
            return obj;
        }
        else
        {
            GameObject obj = Instantiate(prefab, startLocation, Quaternion.identity);
            if (!obj.TryGetComponent<IPoolable>(out var poolable))
            {
                Debug.LogError("Prefab " + prefab.name + " is not poolable and cannot be used.");
                return null;
            }
            poolable.Prefab = prefab;
            return obj;
        }
    }

    public GameObject TakeFromPool(GameObject prefab, Vector3 startLocation, Quaternion startRotation)
    {
        if (!m_Pool.ContainsKey(prefab))
        {
            m_Pool.Add(prefab, new Queue<GameObject>());
        }

        if (m_Pool[prefab].Count > 0)
        {
            GameObject obj = m_Pool[prefab].Dequeue();
            if (obj == null) return TakeFromPool(prefab, startLocation, startRotation);
            obj.transform.SetPositionAndRotation(startLocation, startRotation);
            obj.SetActive(true);
            return obj;
        }
        else
        {
            GameObject obj = Instantiate(prefab, startLocation, startRotation);
            if (!obj.TryGetComponent<IPoolable>(out var poolable))
            {
                Debug.LogError("Prefab " + prefab.name + " is not poolable and cannot be used.");
                return null;
            }
            poolable.Prefab = prefab;
            return obj;
        }
    }

    public void ReturnToPool(GameObject obj)
    {
        if (!obj.TryGetComponent<IPoolable>(out var poolItem))
        {
            Debug.LogError("Object " + obj.name + " was not created from the object pool and cannot be returned.");
            return;
        }
        obj.SetActive(false);
        m_Pool[poolItem.Prefab].Enqueue(obj);
    }
}
