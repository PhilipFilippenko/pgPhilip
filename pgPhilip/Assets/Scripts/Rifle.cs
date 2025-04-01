using UnityEngine;

public class Rifle : WeaponBase
{
    public float bulletSpawnOffset = 1f;

    void Start()
    {
        weaponName = "Rifle";
        ammo = 24;
        maxAmmo = ammo;
        fireRate = 0.05f;
    }

    public override bool Shoot()
    {
        Vector3 shootDirection = transform.root.forward * bulletSpawnOffset;
        return TryShoot(shootDirection);
    }
}
