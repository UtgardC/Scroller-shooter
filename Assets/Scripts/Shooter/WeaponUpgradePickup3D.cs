using UnityEngine;

public class WeaponUpgradePickup3D : PickupBase3D
{
    [SerializeField] private int upgradeAmount = 1;

    private void OnValidate()
    {
        upgradeAmount = Mathf.Max(1, upgradeAmount);
    }

    protected override bool Collect(Collider playerCollider)
    {
        PlayerAutoFire3D playerWeapon = playerCollider.GetComponentInParent<PlayerAutoFire3D>();

        if (playerWeapon == null)
        {
            return false;
        }

        playerWeapon.AddWeaponLevel(upgradeAmount);
        return true;
    }
}
