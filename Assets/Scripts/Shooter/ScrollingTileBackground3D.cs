using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScrollingTileBackground3D : MonoBehaviour
{
    [SerializeField] private GameObject[] tilePrefabs;
    [SerializeField] private int initialTileCount = 4;
    [SerializeField] private float scrollSpeed = 6f;
    [SerializeField] private float recycleZ = -20f;
    [SerializeField] private bool randomizeTiles;
    [SerializeField] private Transform tileParent;

    private readonly List<GroundTile3D> activeTiles = new List<GroundTile3D>();
    private GameObject queuedFinishTilePrefab;
    private FinishGroundTile3D activeFinishTile;
    private Coroutine smoothStopRoutine;
    private bool isSmoothStopping;

    public FinishGroundTile3D ActiveFinishTile => activeFinishTile;
    public bool IsSmoothStopping => isSmoothStopping;

    private void Start()
    {
        SpawnInitialTiles();
    }

    private void Update()
    {
        if (activeTiles.Count == 0 || isSmoothStopping)
        {
            return;
        }

        MoveTiles();
        RecyclePassedTiles();
    }

    private void OnValidate()
    {
        initialTileCount = Mathf.Max(1, initialTileCount);
        scrollSpeed = Mathf.Max(0f, scrollSpeed);
    }

    public void QueueFinishTile(GameObject finishTilePrefab)
    {
        if (finishTilePrefab == null || activeFinishTile != null)
        {
            return;
        }

        queuedFinishTilePrefab = finishTilePrefab;
    }

    public void SmoothStopFinishAt(Transform stopTarget, float duration)
    {
        if (stopTarget == null || activeFinishTile == null)
        {
            return;
        }

        if (smoothStopRoutine != null)
        {
            StopCoroutine(smoothStopRoutine);
        }

        smoothStopRoutine = StartCoroutine(SmoothStopRoutine(stopTarget.position.z, duration));
    }

    private void SpawnInitialTiles()
    {
        activeTiles.Clear();

        if (tilePrefabs == null || tilePrefabs.Length == 0)
        {
            Debug.LogWarning("ScrollingTileBackground3D needs at least one tile prefab.");
            return;
        }

        float nextStartZ = 0f;

        for (int i = 0; i < initialTileCount; i++)
        {
            GroundTile3D tile = CreateTile(ChooseTilePrefab(i));

            if (tile == null)
            {
                continue;
            }

            tile.transform.position = transform.position;

            if (activeTiles.Count > 0)
            {
                tile.PlaceStartAt(nextStartZ);
            }

            nextStartZ = tile.EndZ;
            activeTiles.Add(tile);
        }
    }

    private void MoveTiles()
    {
        Vector3 movement = Vector3.back * (scrollSpeed * Time.deltaTime);

        for (int i = 0; i < activeTiles.Count; i++)
        {
            GroundTile3D tile = activeTiles[i];

            if (tile != null)
            {
                tile.transform.position += movement;
            }
        }
    }

    private void RecyclePassedTiles()
    {
        for (int i = 0; i < activeTiles.Count; i++)
        {
            GroundTile3D tile = activeTiles[i];

            if (tile == null)
            {
                activeTiles.RemoveAt(i);
                i--;
                continue;
            }

            if (tile.EndZ > recycleZ)
            {
                continue;
            }

            float frontEndZ = Mathf.Max(GetFrontEndZ(tile), recycleZ);
            GroundTile3D recycledTile = RecycleTile(tile, frontEndZ, i);

            if (recycledTile == null)
            {
                activeTiles.RemoveAt(i);
                i--;
                continue;
            }

            activeTiles[i] = recycledTile;
        }
    }

    private GroundTile3D RecycleTile(GroundTile3D tile, float targetStartZ, int tileIndex)
    {
        if (activeFinishTile != null)
        {
            if (tile.GetComponent<FinishGroundTile3D>() != activeFinishTile)
            {
                Destroy(tile.gameObject);
                return null;
            }

            return tile;
        }

        if (queuedFinishTilePrefab != null)
        {
            GroundTile3D finishTile = CreateTile(queuedFinishTilePrefab);
            queuedFinishTilePrefab = null;

            if (finishTile != null)
            {
                finishTile.transform.position = tile.transform.position;
                finishTile.PlaceStartAt(targetStartZ);
                activeFinishTile = finishTile.GetComponent<FinishGroundTile3D>();

                if (activeFinishTile == null)
                {
                    Debug.LogWarning($"{finishTile.name} needs a FinishGroundTile3D component.");
                    activeFinishTile = finishTile.gameObject.AddComponent<FinishGroundTile3D>();
                }

                Destroy(tile.gameObject);
                return finishTile;
            }
        }

        if (randomizeTiles)
        {
            GroundTile3D replacement = CreateTile(ChooseTilePrefab(tileIndex));

            if (replacement != null)
            {
                replacement.transform.position = tile.transform.position;
                replacement.PlaceStartAt(targetStartZ);
                Destroy(tile.gameObject);
                return replacement;
            }
        }

        tile.PlaceStartAt(targetStartZ);
        return tile;
    }

    private float GetFrontEndZ(GroundTile3D ignoredTile)
    {
        float frontEndZ = float.NegativeInfinity;

        for (int i = 0; i < activeTiles.Count; i++)
        {
            GroundTile3D tile = activeTiles[i];

            if (tile != null && tile != ignoredTile)
            {
                frontEndZ = Mathf.Max(frontEndZ, tile.EndZ);
            }
        }

        return frontEndZ;
    }

    private GroundTile3D CreateTile(GameObject prefab)
    {
        if (prefab == null)
        {
            return null;
        }

        Transform parent = tileParent != null ? tileParent : transform;
        GameObject instance = Instantiate(prefab, transform.position, prefab.transform.rotation, parent);
        GroundTile3D tile = instance.GetComponent<GroundTile3D>();

        if (tile == null)
        {
            Debug.LogWarning($"{prefab.name} needs a GroundTile3D component.");
            Destroy(instance);
            return null;
        }

        return tile;
    }

    private IEnumerator SmoothStopRoutine(float targetStopZ, float duration)
    {
        isSmoothStopping = true;

        if (duration <= 0f)
        {
            MoveTilesByZ(targetStopZ - activeFinishTile.StopZ);
            scrollSpeed = 0f;
            isSmoothStopping = false;
            smoothStopRoutine = null;
            yield break;
        }

        float startStopZ = activeFinishTile.StopZ;
        float totalDeltaZ = targetStopZ - startStopZ;
        float previousDeltaZ = 0f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;

            float t = Mathf.Clamp01(elapsed / duration);
            float easedT = Mathf.SmoothStep(0f, 1f, t);
            float currentDeltaZ = totalDeltaZ * easedT;

            MoveTilesByZ(currentDeltaZ - previousDeltaZ);
            previousDeltaZ = currentDeltaZ;

            yield return new WaitForEndOfFrame();
        }

        MoveTilesByZ(totalDeltaZ - previousDeltaZ);
        scrollSpeed = 0f;
        isSmoothStopping = false;
        smoothStopRoutine = null;
    }

    private void MoveTilesByZ(float deltaZ)
    {
        Vector3 movement = new Vector3(0f, 0f, deltaZ);

        for (int i = 0; i < activeTiles.Count; i++)
        {
            GroundTile3D tile = activeTiles[i];

            if (tile != null)
            {
                tile.transform.position += movement;
            }
        }
    }

    private GameObject ChooseTilePrefab(int index)
    {
        if (tilePrefabs == null || tilePrefabs.Length == 0)
        {
            return null;
        }

        if (randomizeTiles)
        {
            return tilePrefabs[Random.Range(0, tilePrefabs.Length)];
        }

        return tilePrefabs[index % tilePrefabs.Length];
    }
}
