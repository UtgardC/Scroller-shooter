using UnityEngine;

public class DropOnDeath3D : MonoBehaviour
{
    [SerializeField] private GameObject dropPrefab;
    [SerializeField, Range(0f, 1f)] private float dropChance = 1f;
    [SerializeField] private Transform dropPoint;

    public void TryDrop()
    {
        if (dropPrefab == null || Random.value > dropChance)
        {
            return;
        }

        Transform spawnPoint = dropPoint != null ? dropPoint : transform;
        Instantiate(dropPrefab, spawnPoint.position, spawnPoint.rotation);
    }
}
