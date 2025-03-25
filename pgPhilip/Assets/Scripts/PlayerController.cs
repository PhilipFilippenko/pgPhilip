using UnityEngine;

public class PlayerController : MonoBehaviour, IHealth
{
    internal int health = 1;
    internal float speed = 8f;
    internal float rotationSpeed = 30f;
    internal Vector3 movementDirection;
    public WeaponBase currentWeapon;
    public Transform weaponHolder;
    private CharacterController controller;
    private WeaponPickup nearbyWeapon;
    private Animator animator;

    void Start()
    {
        animator = GetComponentInChildren<Animator>();
        controller = GetComponent<CharacterController>();
        HideWeaponMaterial();
    }

    void Update()
    {
        HandleMovement();
        RotateTowardsMouse();
        HandleShooting();
        if (Input.GetMouseButtonDown(1))
        {
            ThrowWeapon();
            EquipNearbyWeapon();
        }
    }

    void HandleMovement()
    {
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveZ = Input.GetAxisRaw("Vertical");

        movementDirection = new Vector3(moveX, 0, moveZ).normalized;

        if (movementDirection.magnitude > 0 && controller != null)
        {
            animator.SetBool("isRunning", true);
            controller.Move(movementDirection * speed * Time.deltaTime);

            Vector3 fixedPosition = transform.position;
            fixedPosition.y = 1;
            transform.position = fixedPosition;
        }
        else
        {
            animator.SetBool("isRunning", false);
        }

    }

    void RotateTowardsMouse()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Plane groundPlane = new Plane(Vector3.up, transform.position);
        float rayDistance;

        if (groundPlane.Raycast(ray, out rayDistance))
        {
            Vector3 targetPoint = ray.GetPoint(rayDistance);
            Vector3 direction = (targetPoint - transform.position).normalized;
            direction.y = 0;

            if (direction.magnitude > 0)
            {
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction), rotationSpeed * Time.deltaTime);
            }
        }
    }

    void HandleShooting()
    {
        if (currentWeapon == null) return;

        if (currentWeapon.weaponName == "Rifle")
        {
            if (Input.GetMouseButton(0))
            {
                currentWeapon.Shoot();
                animator.SetBool("isShooting", true);
            }

            if (Input.GetMouseButtonUp(0))
            {
                animator.SetBool("isShooting", false);
            }
        }
        else
        {
            if (Input.GetMouseButtonDown(0))
            {
                currentWeapon.Shoot();
                animator.SetTrigger("shootTrigger");
            }
        }
    }


    public void EquipWeapon(WeaponBase newWeapon)
    {
        if (newWeapon == null) return;

        if (currentWeapon != null)
        {
            Destroy(currentWeapon.gameObject);
        }

        currentWeapon = newWeapon;

        Transform grip = newWeapon.transform.Find("GripPoint");

        newWeapon.transform.SetParent(weaponHolder, false);
        newWeapon.transform.localPosition = -grip.localPosition;
        newWeapon.transform.localRotation = Quaternion.Inverse(grip.localRotation);

        newWeapon.DisableCollider();

        if (currentWeapon.weaponName == "Rifle")
        {
            animator.SetFloat("shootSpeed", 2f);
        }

        else if (currentWeapon.weaponName == "Pistol")
        {
            animator.SetFloat("shootSpeed", 2f);
        }
    }

    public void EquipNearbyWeapon()
    {
        if (nearbyWeapon == null) return;

        GameObject newWeaponGO = Instantiate(nearbyWeapon.weaponPrefab, weaponHolder.position, weaponHolder.rotation);
        WeaponBase newWeapon = newWeaponGO.GetComponent<WeaponBase>();

        if (nearbyWeapon.isUsed)
        {
            newWeapon.ammo = nearbyWeapon.remainingAmmo;
        }

        newWeapon.SetPickupTemplate(nearbyWeapon.gameObject);
        EquipWeapon(newWeapon);

        Destroy(nearbyWeapon.gameObject);
        nearbyWeapon = null;
    }

    public void ThrowWeapon()
    {
        if (currentWeapon == null) return;
        Debug.Log("Throwing weapon");

        Vector3 throwPosition = transform.position + transform.forward * 2f;

        Transform weaponTransform = currentWeapon.transform;

        weaponTransform.SetParent(null);
        weaponTransform.position = throwPosition;

        Rigidbody rb = weaponTransform.GetComponent<Rigidbody>();
        if (rb == null)
        {
            rb = weaponTransform.gameObject.AddComponent<Rigidbody>();
        }

        rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
        currentWeapon.Thrown(rb);
        rb.isKinematic = false;
        rb.AddForce(transform.forward * 1000f + Vector3.up * 200f);
        rb.AddTorque(Random.insideUnitSphere * 300f);

        currentWeapon.EnableCollider();
        currentWeapon = null;
    }

    private void OnTriggerEnter(Collider other)
    {
        WeaponPickup pickup = other.GetComponent<WeaponPickup>();
        if (pickup != null)
        {
            nearbyWeapon = pickup;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        WeaponPickup pickup = other.GetComponent<WeaponPickup>();
        if (pickup != null && pickup == nearbyWeapon)
        {
            nearbyWeapon = null;
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
        gameObject.SetActive(false);
    }

    void HideWeaponMaterial()
    {
        SkinnedMeshRenderer renderer = GetComponentInChildren<SkinnedMeshRenderer>();
        if (renderer == null) return;

        Material[] materials = renderer.materials;

        for (int i = 0; i < materials.Length; i++)
        {
            if (materials[i].name.ToLower().Contains("weapon"))
            {
                materials[i].color = new Color(1, 1, 1, 0);
                materials[i].shader = Shader.Find("Legacy Shaders/Transparent/Diffuse");
            }
        }

        renderer.materials = materials;
    }
}