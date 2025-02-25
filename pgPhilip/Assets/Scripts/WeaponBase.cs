using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class WeaponBase : MonoBehaviour, IWeapon
{
    internal string weaponName;
    public int ammo;
    public int maxAmmo;
    public float fireRate;
    public GameObject bulletPrefab;

    protected float nextFireTime = 0f;

    public abstract void Shoot();

    internal void Start()
    {
        Debug.Log("Picked up " + weaponName);
    }
}