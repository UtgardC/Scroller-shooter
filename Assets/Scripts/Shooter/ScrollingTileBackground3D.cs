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

    private void Start()
    {
        SpawnInitialTiles();
    }

    private void Update()
    {
        if (activeTiles.Count == 0)
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
            activeTiles[i] = RecycleTile(tile, frontEndZ, i);
        }
    }

    private GroundTile3D RecycleTile(GroundTile3D tile, float targetStartZ, int tileIndex)
    {
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
