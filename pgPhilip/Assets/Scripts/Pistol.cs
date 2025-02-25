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
        base.Start();
    }

    public override void Shoot()
    {
        if (ammo > 0)
        {
            nextFireTime = Time.time + fireRate;
            ammo--;
            Debug.Log($"Remaining bullets: {ammo}");

            if (ammo <= 0)
            {
                Debug.Log("No ammo left!");
            }

            Vector3 shootDirection = transform.root.forward;
            Vector3 spawnPosition = transform.root.position + shootDirection * bulletSpawnOffset;

            GameObject bullet = Instantiate(bulletPrefab, spawnPosition, Quaternion.LookRotation(shootDirection));
        }
    }
}
