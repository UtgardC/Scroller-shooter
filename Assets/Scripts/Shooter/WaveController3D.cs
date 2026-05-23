using UnityEngine;

public class WaveController3D : MonoBehaviour
{
    [SerializeField] private EnemySpawnPoint3D[] spawnPoints;

    private void OnEnable()
    {
        ExecuteWave();
    }

    private void Reset()
    {
        CacheSpawnPoints();
    }

    private void OnValidate()
    {
        if (spawnPoints == null || spawnPoints.Length == 0)
        {
            CacheSpawnPoints();
        }
    }

    public void ExecuteWave()
    {
        if (spawnPoints == null || spawnPoints.Length == 0)
        {
            CacheSpawnPoints();
        }

        for (int i = 0; i < spawnPoints.Length; i++)
        {
            EnemySpawnPoint3D spawnPoint = spawnPoints[i];

            if (spawnPoint != null)
            {
                spawnPoint.Execute();
            }
        }
    }

    private void CacheSpawnPoints()
    {
        spawnPoints = GetComponentsInChildren<EnemySpawnPoint3D>();
    }
}
