using UnityEngine;

public class Pistol : WeaponBase
{
    public float bulletSpawnOffset = 1f;

    void Start()
    {
        weaponName = "Pistol";
        ammo = 6;
        maxAmmo = ammo;
        fireRate = 0.15f;
    }

    public override bool Shoot()
    {
        Vector3 shootDirection = transform.root.forward * bulletSpawnOffset;
        return TryShoot(shootDirection);
    }
}
