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

    public override void Shoot()
    {
        if (ammo > 0 && Time.time >= nextFireTime)
        {
            nextFireTime = Time.time + fireRate;
            ammo--;
            print("Ammo left: " + ammo);

            Vector3 shootDirection = transform.root.forward;
            Vector3 spawnPosition = transform.root.position + shootDirection * bulletSpawnOffset;

            GameObject bullet = Instantiate(bulletPrefab, spawnPosition, Quaternion.LookRotation(shootDirection));
        }
    }

    public override void Attack() { }
}
