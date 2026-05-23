using UnityEngine;

public abstract class PickupBase3D : MonoBehaviour
{
    [SerializeField] private LayerMask playerMask = ~0;
    [SerializeField] private bool destroyInsteadOfDisable = true;

    private void OnTriggerEnter(Collider other)
    {
        if (other == null || !IsInPlayerMask(other.gameObject.layer))
        {
            return;
        }

        if (Collect(other))
        {
            Consume();
        }
    }

    protected abstract bool Collect(Collider playerCollider);

    private bool IsInPlayerMask(int layer)
    {
        return (playerMask.value & (1 << layer)) != 0;
    }

    private void Consume()
    {
        if (destroyInsteadOfDisable)
        {
            Destroy(gameObject);
            return;
        }

        gameObject.SetActive(false);
    }
}
