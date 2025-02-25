using Unity.VisualScripting;
using UnityEngine;

public class Rifle : WeaponBase
{
    public float bulletSpawnOffset = 1f;

    void Start()
    {
       
        weaponName = "Rifle";
        ammo = 20;
        maxAmmo = ammo;
        fireRate = 0.1f;
        base.Start();
    }

    public override void Shoot()
    {
        if (ammo > 0 && Time.time >= nextFireTime)
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