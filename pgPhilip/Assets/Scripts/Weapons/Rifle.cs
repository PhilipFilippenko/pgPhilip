using UnityEngine;

public class Rifle : WeaponBase
{
    void Awake()
    {
        weaponName = "Rifle";
        ammo = 24;
        maxAmmo = ammo;
        fireRate = 0.05f;
        attackRange = 20f;
}

    public override bool Shoot()
    {
        Vector3 shootDirection = transform.root.forward * bulletSpawnOffset;
        return TryShoot(shootDirection);
    }
}
