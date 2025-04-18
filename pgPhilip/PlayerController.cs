using UnityEngine;

public class PlayerController : MonoBehaviour, IHealth
{
    internal int health = 1;
    private bool isDead = false;

    private CharacterController controller;
    private Animator animator;

    private PlayerMovement movement;
    private PlayerCombat combat;
    private PlayerWeaponHandler weaponHandler;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponentInChildren<Animator>();

        movement = GetComponent<PlayerMovement>();
        combat = GetComponent<PlayerCombat>();
        weaponHandler = GetComponent<PlayerWeaponHandler>();

        combat.Init(animator);
        weaponHandler.Init(animator);
    }

    void Update()
    {
        if (isDead) return;

        movement.HandleMovement();
        movement.RotateTowardsMouse();
        combat.HandleCombat();

        if (Input.GetMouseButtonDown(1))
        {
            weaponHandler.ThrowWeapon();
            weaponHandler.EquipNearbyWeapon();
        }
    }

    public void TakeDamage()
    {
        health--;
        if (health <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        if (isDead) return;
        isDead = true;

        if (controller != null)
            controller.enabled = false;

        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb == null)
            rb = gameObject.AddComponent<Rigidbody>();

        rb.constraints = RigidbodyConstraints.FreezeRotation;
        rb.mass = 50f;

        Vector3 knockbackDir = -transform.forward;
        rb.AddForce(knockbackDir * 10f, ForceMode.Impulse);

        if (animator != null)
            animator.Play("Death");

        this.enabled = false;
    }
}