using UnityEngine;

public class Knife : WeaponBase
{
    void Start()
    {
        weaponName = "Knife";
        ammo = 0;
        maxAmmo = 0;
        fireRate = 0f;
    }

    public override bool Shoot()
    {
        return false;
    }

    public override void Attack()
    {
        GameObject playerObj = GameObject.FindWithTag("Player");
        if (playerObj == null) return;

        float distance = (transform.position - playerObj.transform.position).sqrMagnitude;
        if (distance < 4f)
        {
            if (playerObj.TryGetComponent<IHealth>(out IHealth health))
            {
                health.TakeDamage();
            }
        }
    }
}
