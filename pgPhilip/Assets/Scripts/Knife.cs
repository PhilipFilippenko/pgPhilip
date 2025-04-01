using UnityEngine;

public class Knife : WeaponBase
{
    void Start()
    {
        weaponName = "Knife";
        ammo = 0;
        maxAmmo = 0;
        fireRate = 0f;
        attackRange = 1.7f;
    }

    public override bool Attack()
    {
        if (transform.root.TryGetComponent<PlayerController>(out var player))
        {
            GameObject obj = GameObject.FindWithTag("Enemy");
            if (obj && obj.TryGetComponent<IHealth>(out var enemy))
            {
                TryAttack(enemy);
                return true;
            }
        }
        else if (transform.root.TryGetComponent<EnemyBase>(out var enemy))
        {
            GameObject obj = GameObject.FindWithTag("Player");
            if (obj && obj.TryGetComponent<IHealth>(out var playerTarget))
            {
                TryAttack(playerTarget);
                return true;
            }
        }

        return false;
    }
}
