using UnityEngine;

public class ScrollingBackground : MonoBehaviour
{
    [SerializeField] private Transform[] tiles;
    [SerializeField] private Camera targetCamera;
    [SerializeField] private float scrollSpeed = 6f;
    [SerializeField] private float tileLength = 20f;
    [SerializeField] private float recyclePadding = 2f;

    private void Awake()
    {
        if (targetCamera == null)
        {
            targetCamera = Camera.main;
        }

        if (tiles == null || tiles.Length == 0)
        {
            CacheChildTiles();
        }
    }

    private void Update()
    {
        if (tiles == null || tiles.Length == 0)
        {
            return;
        }

        float moveAmount = scrollSpeed * Time.deltaTime;

        for (int i = 0; i < tiles.Length; i++)
        {
            if (tiles[i] != null && tiles[i] != transform)
            {
                tiles[i].position += Vector3.back * moveAmount;
            }
        }

        RecycleTiles();
    }

    private void RecycleTiles()
    {
        float bottomZ = GetCameraBottomZ();
        float furthestZ = GetFurthestTileZ();

        for (int i = 0; i < tiles.Length; i++)
        {
            Transform tile = tiles[i];

            if (tile == null || tile == transform)
            {
                continue;
            }

            if (tile.position.z + tileLength * 0.5f < bottomZ - recyclePadding)
            {
                tile.position = new Vector3(tile.position.x, tile.position.y, furthestZ + tileLength);
                furthestZ = tile.position.z;
            }
        }
    }

    private float GetCameraBottomZ()
    {
        if (targetCamera == null || !TryGetCameraZLimits(out float minZ, out _))
        {
            return transform.position.z - tileLength;
        }

        return minZ;
    }

    private float GetFurthestTileZ()
    {
        float furthestZ = float.NegativeInfinity;

        for (int i = 0; i < tiles.Length; i++)
        {
            Transform tile = tiles[i];

            if (tile != null && tile != transform)
            {
                furthestZ = Mathf.Max(furthestZ, tile.position.z);
            }
        }

        return furthestZ;
    }

    private void CacheChildTiles()
    {
        tiles = new Transform[transform.childCount];

        for (int i = 0; i < transform.childCount; i++)
        {
            tiles[i] = transform.GetChild(i);
        }
    }

    private bool TryGetCameraZLimits(out float minZ, out float maxZ)
    {
        Plane groundPlane = new Plane(Vector3.up, transform.position);
        minZ = float.PositiveInfinity;
        maxZ = float.NegativeInfinity;

        return TryAddViewportCorner(groundPlane, new Vector3(0f, 0f, 0f), ref minZ, ref maxZ) &&
               TryAddViewportCorner(groundPlane, new Vector3(0f, 1f, 0f), ref minZ, ref maxZ) &&
               TryAddViewportCorner(groundPlane, new Vector3(1f, 0f, 0f), ref minZ, ref maxZ) &&
               TryAddViewportCorner(groundPlane, new Vector3(1f, 1f, 0f), ref minZ, ref maxZ);
    }

    private bool TryAddViewportCorner(Plane plane, Vector3 viewportPoint, ref float minZ, ref float maxZ)
    {
        Ray ray = targetCamera.ViewportPointToRay(viewportPoint);

        if (!plane.Raycast(ray, out float distance))
        {
            return false;
        }

        float z = ray.GetPoint(distance).z;
        minZ = Mathf.Min(minZ, z);
        maxZ = Mathf.Max(maxZ, z);
        return true;
    }
}
