using UnityEngine;

public class Pistol : WeaponBase
{
    void Awake()
    {
        weaponName = "Pistol";
        ammo = 6;
        maxAmmo = ammo;
        fireRate = 0.2f;
        attackRange = 20f;
    }

    public override bool Shoot()
    {
        Vector3 shootDirection = transform.root.forward * bulletSpawnOffset;
        return TryShoot(shootDirection);
    }
}
