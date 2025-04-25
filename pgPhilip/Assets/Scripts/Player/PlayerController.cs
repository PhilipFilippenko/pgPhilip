using UnityEngine;

public class PlayerController : MonoBehaviour, IHealth
{
    internal int health = 1;
    internal float speed = 8f;
    internal float rotationSpeed = 30f;
    internal Vector3 movementDirection;

    internal bool isDead = false;

    private float meleeAttackCooldown = 0.2f;

    public WeaponBase currentWeapon;
    public Transform WeaponHolder;
    public Animator Animator => animator;

    private CharacterController controller;
    private WeaponPickup nearbyWeapon;
    private Animator animator;

    private PlayerMovement movement;
    private PlayerCombat combat;
    private PlayerWeaponHandler weaponHandler;

    void Start()
    {
        animator = GetComponentInChildren<Animator>();
        controller = GetComponent<CharacterController>();
        movement = GetComponent<PlayerMovement>();
        combat = GetComponent<PlayerCombat>();
        weaponHandler = GetComponent<PlayerWeaponHandler>();

        combat.Initialize(this, animator, meleeAttackCooldown);
        weaponHandler.Initialize(this);
    }

    void Update()
    {
        if (isDead) return;

        movement.HandleMovement();
        movement.RotateTowardsMouse();
        combat.HandleShooting();

        if (Input.GetMouseButtonDown(1))
        {
            weaponHandler.ThrowWeapon();
            weaponHandler.EquipNearbyWeapon();
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (currentWeapon != null)
            {
                weaponHandler.ThrowWeapon();
            }

            combat.TryStartExecution();
        }
    }

    public void TakeDamage()
    {
        health--;
        if (health <= 0) Die();
    }

    private void Die()
    {
        if (isDead) return;

        isDead = true;
        controller.enabled = false;

        if (!TryGetComponent<Rigidbody>(out var rb))
        {
            rb = gameObject.AddComponent<Rigidbody>();
        }

        rb.constraints = RigidbodyConstraints.FreezeRotation;
        rb.mass = 10f;
        rb.isKinematic = false;

        rb.AddForce(-transform.forward * 50f + Vector3.up * 10f, ForceMode.Impulse);
        animator?.Play("Death");

        enabled = false;
        gameObject.layer = LayerMask.NameToLayer("DeadBody");
    }

    public WeaponPickup GetNearbyWeapon() => nearbyWeapon;
    public void SetNearbyWeapon(WeaponPickup pickup) => nearbyWeapon = pickup;
}
