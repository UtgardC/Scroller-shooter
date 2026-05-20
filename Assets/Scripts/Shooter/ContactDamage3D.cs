using UnityEngine;

public class ContactDamage3D : MonoBehaviour
{
    [SerializeField] private int damage = 1;
    [SerializeField] private LayerMask targetMask = ~0;

    private void OnTriggerEnter(Collider other)
    {
        TryDamage(other);
    }

    private void OnCollisionEnter(Collision collision)
    {
        TryDamage(collision.collider);
    }

    private void OnValidate()
    {
        damage = Mathf.Max(0, damage);
    }

    private void TryDamage(Collider other)
    {
        if (other == null || !IsInTargetMask(other.gameObject.layer))
        {
            return;
        }

        Health health = other.GetComponentInParent<Health>();

        if (health != null)
        {
            health.TakeDamage(damage);
            return;
        }

        other.SendMessageUpwards("TakeDamage", damage, SendMessageOptions.DontRequireReceiver);
    }

    private bool IsInTargetMask(int layer)
    {
        return (targetMask.value & (1 << layer)) != 0;
    }
}
