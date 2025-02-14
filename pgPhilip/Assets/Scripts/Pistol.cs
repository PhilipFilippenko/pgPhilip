using UnityEngine;

public class Pistol : WeaponBase
{
    public float bulletSpawnOffset = 1f;

    void Start()
    {
        weaponName = "Pistol";
        ammo = 6;
        fireRate = 0.15f;
    }

    public override void Shoot()
    {
        if (ammo > 0)
        {
            nextFireTime = Time.time + fireRate;
            ammo--;

            Vector3 shootDirection = transform.root.forward;
            Vector3 spawnPosition = transform.root.position + shootDirection * bulletSpawnOffset;

            GameObject bullet = Instantiate(bulletPrefab, spawnPosition, Quaternion.LookRotation(shootDirection));
        }
    }
}
