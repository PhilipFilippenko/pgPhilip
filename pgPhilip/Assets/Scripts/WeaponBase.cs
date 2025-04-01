using UnityEngine;

public abstract class WeaponBase : MonoBehaviour
{
    public string weaponName;
    public int ammo;
    public int maxAmmo;
    public float fireRate;
    public float attackRange = 1.5f;
    public GameObject bulletPrefab;
    internal float throwDamage = 1f;

    protected float nextFireTime = 0f;
    private Collider weaponCollider;
    private bool isThrown = false;
    private Rigidbody rb;
    private float timeSinceLastMovement = 0.1f;
    private float movementThreshold = 0.1f;
    private GameObject pickupTemplate;

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

    public void DisableCollider()
    {
        if (weaponCollider != null)
        {
            weaponCollider.enabled = false;
        }
    }

    void Update()
    {
        if (isThrown && rb != null)
        {
            if (rb.velocity.magnitude < movementThreshold)
                timeSinceLastMovement -= Time.deltaTime;

            if (timeSinceLastMovement <= 0)
                ConvertToPickup();
        }
    }

    private void ConvertToPickup()
    {
        if (pickupTemplate != null)
        {
            GameObject pickupInstance = Instantiate(pickupTemplate, transform.position, Quaternion.identity);
            pickupInstance.SetActive(true);
        }
        Destroy(gameObject);
    }

    public void EnableCollider()
    {
        if (weaponCollider != null)
        {
            weaponCollider.enabled = true;
        }
    }

    internal void Thrown(Rigidbody newRB)
    {
        isThrown = true;
        rb = newRB;
        timeSinceLastMovement = 0.1f;
    }

    internal void SetPickupTemplate(GameObject template)
    {
        pickupTemplate = Instantiate(template);
        pickupTemplate.SetActive(false);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!isThrown) return;

        if (collision.gameObject.TryGetComponent<IHealth>(out IHealth entity))
        {
            if (!(entity is PlayerController))
            {
                entity.TakeDamage();
                Debug.Log("Weapon hit an enemy!");
            }

            if (rb != null)
                rb.velocity = new Vector3(0, rb.velocity.y, 0);
        }
    }

    protected bool TryShoot(Vector3 shootDirection)
    {
        if (ammo > 0 && Time.time >= nextFireTime)
        {
            nextFireTime = Time.time + fireRate;
            ammo--;

            Vector3 spawnPosition = transform.root.position + shootDirection;
            Instantiate(bulletPrefab, spawnPosition, Quaternion.LookRotation(shootDirection));
            return true;
        }
        return false;
    }

    protected void TryAttack(IHealth target)
    {
        if (target == null) return;

        if (target is Component targetComponent)
        {
            float dist = (targetComponent.transform.position - transform.position).sqrMagnitude;
            if (dist <= attackRange * attackRange)
            {
                target.TakeDamage();
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }

}
