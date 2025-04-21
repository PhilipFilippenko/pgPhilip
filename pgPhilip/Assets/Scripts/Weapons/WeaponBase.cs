using UnityEngine;

public abstract class WeaponBase : MonoBehaviour
{
    internal string weaponName;
    internal int ammo;
    internal int maxAmmo;
    internal float fireRate;
    internal float attackRange;
    internal float bulletSpawnOffset = 1f;
    internal bool isMelee = false;
    internal bool infiniteAmmo = false;
    public GameObject bulletPrefab;
    public GameObject floatingWeaponPrefab;

    protected float nextFireTime = 0f;

    private Collider weaponCollider;
    private bool isThrown = false;
    private Rigidbody rb;
    private float timeSinceLastMovement = 0.1f;
    private float movementThreshold = 0.1f;

    private GameObject pickupTemplate;
    private WeaponData weaponData;

    void Awake()
    {
        weaponCollider = GetComponent<Collider>();
    }

    public virtual bool Shoot()
    {
        return false;
    }

    public virtual bool Attack()
    {
        return false;
    }

    public GameObject GetFloatingWeaponPrefab()
    {
        return floatingWeaponPrefab;
    }

    public bool ShootAt(Vector3 direction)
    {
        return TryShoot(direction);
    }

    public void DisableCollider()
    {
        if (weaponCollider != null)
        {
            weaponCollider.enabled = false;
        }
    }

    public void EnableCollider()
    {
        if (weaponCollider != null)
        {
            weaponCollider.enabled = true;
        }
    }

    void Update()
    {
        if (isThrown && rb != null)
        {
            if (rb.velocity.magnitude < movementThreshold)
            {
                timeSinceLastMovement -= Time.deltaTime;
            }

            if (timeSinceLastMovement <= 0)
            {
                ConvertToPickup();
            }
        }
    }

    private void ConvertToPickup()
    {
        Vector3 spawnPos = transform.position;
        spawnPos.y = 1f;

        GameObject pickupInstance = Instantiate(pickupTemplate, spawnPos, Quaternion.identity);
        pickupInstance.name = pickupTemplate.name.Replace("(Clone)", "").Trim();
        pickupInstance.SetActive(true);

        pickupInstance.TryGetComponent(out WeaponPickup pickup);
        pickup.UpdateUsedStatus(weaponData.ammo);

        Destroy(pickupTemplate);
        Destroy(gameObject);
    }

    internal void Thrown(Rigidbody newRB)
    {
        isThrown = true;
        rb = newRB;

        rb.useGravity = true;
        rb.drag = 1f;
        rb.angularDrag = 2f;

        timeSinceLastMovement = 0.1f;

        if (weaponCollider == null)
        {
            weaponCollider = GetComponent<Collider>();
            if (weaponCollider == null)
            {
                weaponCollider = gameObject.AddComponent<BoxCollider>();
            }
        }

        weaponCollider.enabled = true;

        if (weaponData != null)
        {
            weaponData.ammo = ammo;
        }
    }

    internal void SetPickupTemplate(GameObject template)
    {
        pickupTemplate = Instantiate(template);
        pickupTemplate.SetActive(false);
        weaponData = new WeaponData(ammo);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!isThrown) return;

        if (collision.gameObject.TryGetComponent(out IHealth entity))
        {
            if (!(entity is PlayerController))
            {
                entity.TakeDamage();
            }

            rb.velocity = new Vector3(0, rb.velocity.y, 0);
        }
    }

    protected bool TryShoot(Vector3 shootDirection)
    {
        if ((ammo > 0 || infiniteAmmo) && Time.time >= nextFireTime)
        {
            nextFireTime = Time.time + fireRate;

            if (!infiniteAmmo)
                ammo--;

            Vector3 spawnPosition = transform.root.position + shootDirection;
            spawnPosition.y = 1f;

            Instantiate(bulletPrefab, spawnPosition, Quaternion.LookRotation(shootDirection));
            return true;
        }

        return false;
    }

    protected bool TryAttack(Vector3 origin, float radius)
    {
        Collider[] hits = Physics.OverlapSphere(origin, radius);
        foreach (var hit in hits)
        {
            if (hit.TryGetComponent<IHealth>(out var target))
            {
                if (target == transform.root.GetComponent<IHealth>()) continue;
                if (hit.CompareTag(gameObject.tag)) continue;

                if (target is Component targetComponent)
                {
                    float dist = (targetComponent.transform.position - transform.position).sqrMagnitude;
                    if (dist <= attackRange * attackRange)
                    {
                        target.TakeDamage();
                        return true;
                    }
                }
            }
        }

        return false;
    }
}
