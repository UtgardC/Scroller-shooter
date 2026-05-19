using System.Collections.Generic;
using UnityEngine;

public class PoolManager : MonoBehaviour
{
    public static PoolManager Instance { get; private set; }

    private readonly Dictionary<GameObject, Queue<Poolable>> pools = new Dictionary<GameObject, Queue<Poolable>>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.LogWarning("More than one PoolManager exists in the scene. The newest one will be used.");
        }

        Instance = this;
    }

    public GameObject Get(GameObject prefab, Vector3 position, Quaternion rotation)
    {
        if (prefab == null)
        {
            return null;
        }

        Poolable poolable = GetAvailablePoolable(prefab);
        Transform poolableTransform = poolable.transform;

        poolableTransform.SetPositionAndRotation(position, rotation);
        poolableTransform.SetParent(null);
        poolable.MarkSpawned();
        poolable.gameObject.SetActive(true);

        return poolable.gameObject;
    }

    public void ReturnToPool(Poolable poolable)
    {
        if (poolable == null || poolable.IsInPool)
        {
            return;
        }

        GameObject prefab = poolable.SourcePrefab;

        if (prefab == null)
        {
            poolable.gameObject.SetActive(false);
            return;
        }

        if (!pools.TryGetValue(prefab, out Queue<Poolable> pool))
        {
            pool = new Queue<Poolable>();
            pools.Add(prefab, pool);
        }

        poolable.gameObject.SetActive(false);
        poolable.transform.SetParent(transform);
        poolable.MarkReturned();
        pool.Enqueue(poolable);
    }

    private Poolable GetAvailablePoolable(GameObject prefab)
    {
        if (!pools.TryGetValue(prefab, out Queue<Poolable> pool))
        {
            pool = new Queue<Poolable>();
            pools.Add(prefab, pool);
        }

        while (pool.Count > 0)
        {
            Poolable poolable = pool.Dequeue();

            if (poolable != null)
            {
                return poolable;
            }
        }

        return CreatePoolable(prefab);
    }

    private Poolable CreatePoolable(GameObject prefab)
    {
        GameObject instance = Instantiate(prefab, transform);
        Poolable poolable = instance.GetComponent<Poolable>();

        if (poolable == null)
        {
            poolable = instance.AddComponent<Poolable>();
        }

        poolable.Initialize(this, prefab);
        poolable.MarkReturned();
        instance.SetActive(false);

        return poolable;
    }
}
