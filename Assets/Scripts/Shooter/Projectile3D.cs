using UnityEngine;

[RequireComponent(typeof(Poolable))]
public class Projectile3D : MonoBehaviour
{
    [SerializeField] private float speed = 18f;
    [SerializeField] private int damage = 1;
    [SerializeField] private float lifeTime = 3f;
    [SerializeField] private LayerMask hitMask = ~0;

    private Poolable poolable;
    private Vector3 moveDirection = Vector3.forward;
    private float despawnTime;

    private void Awake()
    {
        poolable = GetComponent<Poolable>();
        EnsureTriggerCollider();
    }

    private void OnEnable()
    {
        despawnTime = Time.time + lifeTime;
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

        Health health = other.GetComponentInParent<Health>();

        if (health != null)
        {
            health.TakeDamage(damage);
        }

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
}
