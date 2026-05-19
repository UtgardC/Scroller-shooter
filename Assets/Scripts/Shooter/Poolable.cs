using UnityEngine;

public class Poolable : MonoBehaviour
{
    private PoolManager poolManager;
    private GameObject sourcePrefab;
    private bool isInPool;

    public GameObject SourcePrefab => sourcePrefab;
    public bool IsInPool => isInPool;

    internal void Initialize(PoolManager manager, GameObject prefab)
    {
        poolManager = manager;
        sourcePrefab = prefab;
    }

    internal void MarkSpawned()
    {
        isInPool = false;
    }

    internal void MarkReturned()
    {
        isInPool = true;
    }

    public void ReturnToPool()
    {
        if (poolManager != null)
        {
            poolManager.ReturnToPool(this);
            return;
        }

        gameObject.SetActive(false);
    }
}
