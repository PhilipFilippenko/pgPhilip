using Assets.Scripts.Enemies;
using System.Collections;
using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    private PlayerController player;
    private Animator animator;
    private CharacterController controller;
    private float meleeCooldown;
    private bool canAttack = true;

    private EnemyBase executingTarget;
    private bool isExecuting = false;
    private int executionStep = 0;

    public void Initialize(PlayerController p, Animator a, float cooldown)
    {
        player = p;
        animator = a;
        meleeCooldown = cooldown;
        controller = player.GetComponent<CharacterController>();
    }

    public void HandleShooting()
    {
        if (isExecuting && Input.GetMouseButtonDown(0) && executingTarget != null)
        {
            animator.Play("MeleeAttack_OneHanded", 0, 0f);
            executingTarget.ReceiveExecutionHit();
            executionStep++;

            if (executionStep >= 3)
            {
                isExecuting = false;
                executingTarget = null;
                controller.enabled = true;
            }

            return;
        }

        if (player.currentWeapon == null)
            return;

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

    public void TryStartExecution()
    {
        if (isExecuting) return;

        Collider[] hits = Physics.OverlapSphere(player.transform.position, 2f);
        foreach (var hit in hits)
        {
            if (hit.TryGetComponent<EnemyBase>(out var enemy) && enemy.IsStunned() && !enemy.IsDead())
            {
                Vector3 enemyForward = enemy.transform.forward;
                Vector3 targetPosition = enemy.transform.position - enemyForward * 0.6f;
                targetPosition.y = player.transform.position.y;

                player.transform.position = targetPosition;
                player.transform.rotation = Quaternion.LookRotation(enemyForward);

                controller.enabled = false;

                executingTarget = enemy;
                isExecuting = true;
                executionStep = 0;
                enemy.StartExecution();
                break;
            }
        }
    }

    private IEnumerator ResetCooldown()
    {
        yield return new WaitForSeconds(meleeCooldown);
        canAttack = true;
    }
}
