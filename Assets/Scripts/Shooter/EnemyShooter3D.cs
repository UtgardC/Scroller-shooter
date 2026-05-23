using UnityEngine;

public class EnemyShooter3D : MonoBehaviour
{
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private Transform[] firePoints;
    [SerializeField] private float fireRate = 1f;
    [SerializeField] private float initialDelay = 0.5f;

    private float nextFireTime;
    private bool missingPoolWarningShown;

    private void OnEnable()
    {
        nextFireTime = Time.time + initialDelay;
        missingPoolWarningShown = false;
    }

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

    private void OnValidate()
    {
        fireRate = Mathf.Max(0f, fireRate);
        initialDelay = Mathf.Max(0f, initialDelay);
    }

    private void Fire()
    {
        PoolManager poolManager = PoolManager.Instance;

        if (poolManager == null)
        {
            if (!missingPoolWarningShown)
            {
                Debug.LogWarning("EnemyShooter3D needs a PoolManager in the scene.");
                missingPoolWarningShown = true;
            }

            return;
        }

        for (int i = 0; i < firePoints.Length; i++)
        {
            Transform firePoint = firePoints[i];

            if (firePoint == null)
            {
                continue;
            }

            Vector3 direction = firePoint.forward;
            GameObject projectileObject = poolManager.Get(projectilePrefab, firePoint.position, firePoint.rotation);

            if (projectileObject != null && projectileObject.TryGetComponent(out Projectile3D projectile))
            {
                projectile.SetDirection(direction);
            }
        }
    }
}
