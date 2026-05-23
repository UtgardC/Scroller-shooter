using UnityEngine;

[RequireComponent(typeof(Poolable))]
public class Projectile3D : MonoBehaviour
{
    [SerializeField] private float speed = 18f;
    [SerializeField] private int damage = 1;
    [SerializeField] private float lifeTime = 3f;
    [SerializeField] private LayerMask hitMask = ~0;
    [SerializeField] private TrailRenderer trail;

    private Poolable poolable;
    private Vector3 moveDirection = Vector3.forward;
    private float despawnTime;

    private void Awake()
    {
        poolable = GetComponent<Poolable>();

        if (trail == null)
        {
            trail = GetComponentInChildren<TrailRenderer>();
        }

        EnsureTriggerCollider();
    }

    private void OnEnable()
    {
        despawnTime = Time.time + lifeTime;
        ResetTrail();
    }

    private void OnValidate()
    {
        EnsureTriggerCollider();
    }

    private void EnsureTriggerCollider()
    {
        Collider projectileCollider = GetComponent<Collider>();

        if (projectileCollider != null)
        {
            projectileCollider.isTrigger = true;
        }
    }

    private void Update()
    {
        transform.position += moveDirection * (speed * Time.deltaTime);

        if (Time.time >= despawnTime)
        {
            ReturnToPool();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!IsInHitMask(other.gameObject.layer))
        {
            return;
        }

        PlayerHealth playerHealth = other.GetComponentInParent<PlayerHealth>();

        if (playerHealth != null)
        {
            playerHealth.TakeDamage(damage);
            ReturnToPool();
            return;
        }

        Health health = other.GetComponentInParent<Health>();

        if (health != null)
        {
            health.TakeDamage(damage);
            ReturnToPool();
            return;
        }

        other.SendMessageUpwards("TakeDamage", damage, SendMessageOptions.DontRequireReceiver);
        ReturnToPool();
    }

    public void SetDirection(Vector3 direction)
    {
        if (direction.sqrMagnitude <= 0f)
        {
            moveDirection = Vector3.forward;
            return;
        }

        moveDirection = direction.normalized;
    }

    private bool IsInHitMask(int layer)
    {
        return (hitMask.value & (1 << layer)) != 0;
    }

    private void ReturnToPool()
    {
        StopTrail();

        if (poolable == null)
        {
            poolable = GetComponent<Poolable>();
        }

        if (poolable != null)
        {
            poolable.ReturnToPool();
            return;
        }

        gameObject.SetActive(false);
    }

    private void ResetTrail()
    {
        if (trail == null)
        {
            return;
        }

        trail.enabled = true;
        trail.Clear();
        trail.emitting = true;
    }

    private void StopTrail()
    {
        if (trail == null)
        {
            return;
        }

        trail.emitting = false;
        trail.Clear();
        trail.enabled = false;
    }
}
