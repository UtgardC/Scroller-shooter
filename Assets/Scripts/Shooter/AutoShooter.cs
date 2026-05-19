using UnityEngine;

public class AutoShooter : MonoBehaviour
{
    [SerializeField] private Projectile projectilePrefab;
    [SerializeField] private Transform firePoint;
    [SerializeField] private float shotsPerSecond = 6f;
    [SerializeField] private float muzzleOffset = 1.2f;

    private float nextShotTime;

    private void Update()
    {
        if (projectilePrefab == null || Time.time < nextShotTime)
        {
            return;
        }

        Shoot();
        nextShotTime = Time.time + 1f / shotsPerSecond;
    }

    private void Shoot()
    {
        Vector3 spawnPosition = firePoint != null
            ? firePoint.position
            : transform.position + transform.forward * muzzleOffset;

        Quaternion spawnRotation = firePoint != null
            ? firePoint.rotation
            : transform.rotation;

        Instantiate(projectilePrefab, spawnPosition, spawnRotation);
    }
}
