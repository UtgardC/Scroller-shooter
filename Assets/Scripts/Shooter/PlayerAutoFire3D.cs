using UnityEngine;

public class PlayerAutoFire3D : MonoBehaviour
{
    [System.Serializable]
    public class FirePointGroup
    {
        public Transform[] firePoints;
    }

    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private FirePointGroup[] firePointGroups;
    [SerializeField] private int startingWeaponLevel = 1;
    [SerializeField] private float fireRate = 6f;

    private float nextFireTime;
    private int currentWeaponLevel;
    private bool missingPoolWarningShown;

    public int CurrentWeaponLevel => currentWeaponLevel;
    public int MaxWeaponLevel => GetConfiguredMaxWeaponLevel();

    private void OnEnable()
    {
        currentWeaponLevel = Mathf.Max(1, startingWeaponLevel);
        ClampWeaponLevel();
    }

    private void Update()
    {
        Transform[] activeFirePoints = GetActiveFirePoints();

        if (projectilePrefab == null || activeFirePoints == null || activeFirePoints.Length == 0 || fireRate <= 0f)
        {
            return;
        }

        if (Time.time < nextFireTime)
        {
            return;
        }

        Fire(activeFirePoints);
        nextFireTime = Time.time + 1f / fireRate;
    }

    private void OnValidate()
    {
        startingWeaponLevel = Mathf.Max(1, startingWeaponLevel);
        fireRate = Mathf.Max(0f, fireRate);
    }

    public void AddWeaponLevel(int amount)
    {
        if (amount <= 0)
        {
            return;
        }

        currentWeaponLevel += amount;
        ClampWeaponLevel();
    }

    public void SetWeaponLevel(int level)
    {
        currentWeaponLevel = Mathf.Max(1, level);
        ClampWeaponLevel();
    }

    private void Fire(Transform[] activeFirePoints)
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

        for (int i = 0; i < activeFirePoints.Length; i++)
        {
            Transform firePoint = activeFirePoints[i];

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

    private Transform[] GetActiveFirePoints()
    {
        if (firePointGroups == null || firePointGroups.Length == 0)
        {
            return null;
        }

        int groupIndex = Mathf.Clamp(currentWeaponLevel, 1, firePointGroups.Length) - 1;
        FirePointGroup group = firePointGroups[groupIndex];
        return group != null ? group.firePoints : null;
    }

    private int GetConfiguredMaxWeaponLevel()
    {
        if (firePointGroups != null && firePointGroups.Length > 0)
        {
            return firePointGroups.Length;
        }

        return 1;
    }

    private void ClampWeaponLevel()
    {
        currentWeaponLevel = Mathf.Clamp(currentWeaponLevel, 1, GetConfiguredMaxWeaponLevel());
    }
}
