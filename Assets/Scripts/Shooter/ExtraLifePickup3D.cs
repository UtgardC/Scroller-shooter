using UnityEngine;

public class ExtraLifePickup3D : PickupBase3D
{
    [SerializeField] private int lifeAmount = 1;

    private void OnValidate()
    {
        lifeAmount = Mathf.Max(1, lifeAmount);
    }

    protected override bool Collect(Collider playerCollider)
    {
        PlayerHealth playerHealth = playerCollider.GetComponentInParent<PlayerHealth>();

        if (playerHealth == null)
        {
            return false;
        }

        playerHealth.AddLife(lifeAmount);
        return true;
    }
}
