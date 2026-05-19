using UnityEngine;

public class PlayerAutoFire3D : MonoBehaviour
{
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private Transform[] firePoints;
    [SerializeField] private float fireRate = 6f;
    [SerializeField] private Vector3 fireDirection = Vector3.forward;

    private float nextFireTime;
    private bool missingPoolWarningShown;

    private void Update()
    {
        if (projectilePrefab == null || firePoints == null || firePoints.Length == 0 || fireRate <= 0f)
        {
            return;
        }

        if (Time.time < nextFireTime)
        {
            return;
        }

        Fire();
        nextFireTime = Time.time + 1f / fireRate;
    }

    private void Fire()
    {
        PoolManager poolManager = PoolManager.Instance;

        if (poolManager == null)
        {
            if (!missingPoolWarningShown)
            {
                Debug.LogWarning("PlayerAutoFire3D needs a PoolManager in the scene.");
                missingPoolWarningShown = true;
            }

            return;
        }

        Vector3 direction = fireDirection.sqrMagnitude > 0f ? fireDirection.normalized : Vector3.forward;

        for (int i = 0; i < firePoints.Length; i++)
        {
            Transform firePoint = firePoints[i];

            if (firePoint == null)
            {
                continue;
            }

            GameObject projectileObject = poolManager.Get(projectilePrefab, firePoint.position, Quaternion.LookRotation(direction));

            if (projectileObject != null && projectileObject.TryGetComponent(out Projectile3D projectile))
            {
                projectile.SetDirection(direction);
            }
        }
    }
}
