using System.Collections;
using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    private PlayerController player;
    private Animator animator;
    private float meleeCooldown;
    private bool canAttack = true;

    public void Initialize(PlayerController p, Animator a, float cooldown)
    {
        player = p;
        animator = a;
        meleeCooldown = cooldown;
    }

    public void HandleShooting()
    {
        if (player.currentWeapon == null)
        {
            return;
        }

        string name = player.currentWeapon.weaponName;

        if (name == "Knife")
        {
            if (Input.GetMouseButtonDown(0) && canAttack)
            {
                canAttack = false;
                animator.Play("MeleeAttack_OneHanded", 0, 0f);
                player.currentWeapon.Attack();
                StartCoroutine(ResetCooldown());
            }
        }
        else if (name == "Rifle")
        {
            if (Input.GetMouseButton(0))
            {
                if (player.currentWeapon.Shoot())
                {
                    animator.Play("RifleShoot", 0, 0f);
                }
            }
        }
        else if (name == "Pistol")
        {
            if (Input.GetMouseButtonDown(0))
            {
                if (player.currentWeapon.Shoot())
                {
                    animator.Play("RifleShoot", 0, 0f);
                }
            }
        }
    }

    private IEnumerator ResetCooldown()
    {
        yield return new WaitForSeconds(meleeCooldown);
        canAttack = true;
    }
}