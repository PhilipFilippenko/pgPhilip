using UnityEngine;

public class WeaponMelee : WeaponBase
{
    public float attackRange = 2f;
    public float damage = 1f;
    private PlayerController player;

    private void Start()
    {
        player = FindObjectOfType<PlayerController>();
    }

    public override void Attack()
    {
        if (player == null) return;

        if ((transform.position - player.transform.position).sqrMagnitude <= attackRange * attackRange)
        {
            player.TakeDamage();
        }
    }

    public override bool Shoot()
    {
        return false;
    }
}
