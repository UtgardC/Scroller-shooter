using System.Collections;
using UnityEngine;

public class EnemySpawnPoint3D : MonoBehaviour
{
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private float delay;
    [SerializeField] private float gizmoRadius = 0.35f;

    private Coroutine spawnRoutine;

    private void OnDisable()
    {
        if (spawnRoutine != null)
        {
            StopCoroutine(spawnRoutine);
            spawnRoutine = null;
        }
    }

    private void OnValidate()
    {
        delay = Mathf.Max(0f, delay);
        gizmoRadius = Mathf.Max(0.01f, gizmoRadius);
    }

    public void Execute()
    {
        if (spawnRoutine != null)
        {
            StopCoroutine(spawnRoutine);
        }

        spawnRoutine = StartCoroutine(SpawnRoutine());
    }

    private IEnumerator SpawnRoutine()
    {
        if (delay > 0f)
        {
            yield return new WaitForSeconds(delay);
        }

        Spawn();
        spawnRoutine = null;
    }

    private void Spawn()
    {
        if (enemyPrefab == null)
        {
            return;
        }

        Instantiate(enemyPrefab, transform.position, transform.rotation);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(transform.position, gizmoRadius);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, gizmoRadius * 1.4f);
    }
}
