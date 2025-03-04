using UnityEngine;

public abstract class WeaponBase : MonoBehaviour
{
    public string weaponName;
    public int ammo;
    public int maxAmmo;
    public float fireRate;
    public GameObject bulletPrefab;
    public Transform firePoint;

    protected float nextFireTime = 0f;

    public abstract void Shoot();
}
