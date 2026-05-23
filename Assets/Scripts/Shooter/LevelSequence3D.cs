using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class LevelSequence3D : MonoBehaviour
{
    [System.Serializable]
    public class WaveEntry
    {
        public GameObject wavePrefab;
        public float spawnTime;
    }

    [SerializeField] private WaveEntry[] waves;
    [SerializeField] private Transform basePosition;
    [SerializeField] private Transform waveParent;
    [SerializeField] private float postLastWaveDelay = 3f;
    [SerializeField] private UnityEvent onSequenceComplete = new UnityEvent();
    [SerializeField] private float basePositionGizmoRadius = 0.6f;

    private float startTime;
    private int nextWaveIndex;
    private bool completionScheduled;
    private Coroutine completionRoutine;

    private void OnEnable()
    {
        startTime = Time.time;
        nextWaveIndex = 0;
        completionScheduled = false;
        completionRoutine = null;

        if (waves == null || waves.Length == 0)
        {
            ScheduleSequenceComplete();
        }
    }

    private void OnDisable()
    {
        if (completionRoutine != null)
        {
            StopCoroutine(completionRoutine);
            completionRoutine = null;
        }
    }

    private void Update()
    {
        if (waves == null || nextWaveIndex >= waves.Length)
        {
            ScheduleSequenceComplete();
            return;
        }

        float elapsedTime = Time.time - startTime;

        while (nextWaveIndex < waves.Length)
        {
            WaveEntry entry = waves[nextWaveIndex];

            if (entry == null)
            {
                nextWaveIndex++;
                continue;
            }

            if (elapsedTime < entry.spawnTime)
            {
                break;
            }

            SpawnWave(entry);
            nextWaveIndex++;
        }

        if (nextWaveIndex >= waves.Length)
        {
            ScheduleSequenceComplete();
        }
    }

    private void OnValidate()
    {
        basePositionGizmoRadius = Mathf.Max(0.01f, basePositionGizmoRadius);
        postLastWaveDelay = Mathf.Max(0f, postLastWaveDelay);

        if (waves == null)
        {
            return;
        }

        for (int i = 0; i < waves.Length; i++)
        {
            if (waves[i] != null)
            {
                waves[i].spawnTime = Mathf.Max(0f, waves[i].spawnTime);
            }
        }
    }

    private void SpawnWave(WaveEntry entry)
    {
        if (entry.wavePrefab == null)
        {
            return;
        }

        Transform parent = waveParent != null ? waveParent : transform;
        Transform spawnTransform = basePosition != null ? basePosition : transform;
        Instantiate(entry.wavePrefab, spawnTransform.position, spawnTransform.rotation, parent);
    }

    private void ScheduleSequenceComplete()
    {
        if (completionScheduled)
        {
            return;
        }

        completionScheduled = true;
        completionRoutine = StartCoroutine(SequenceCompleteRoutine());
    }

    private IEnumerator SequenceCompleteRoutine()
    {
        if (postLastWaveDelay > 0f)
        {
            yield return new WaitForSeconds(postLastWaveDelay);
        }

        completionRoutine = null;
        onSequenceComplete?.Invoke();
    }

    private void OnDrawGizmos()
    {
        Transform targetTransform = basePosition != null ? basePosition : transform;
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(targetTransform.position, basePositionGizmoRadius);
    }
}
