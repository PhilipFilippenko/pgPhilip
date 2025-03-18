using System;
using UnityEngine;

public abstract class WeaponBase : MonoBehaviour
{
    public string weaponName;
    public int ammo;
    public int maxAmmo;
    public float fireRate;
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

    public abstract void Shoot();
    public abstract void Attack();

    public void DisableCollider()
    {
        if (weaponCollider != null)
        {
            weaponCollider.enabled = false;
        }
    }

    private void Update()
    {
        if (isThrown)
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
            if (entity is PlayerController)
            {
                return;
            }

            entity.TakeDamage();
            Debug.Log("Weapon hit an enemy!");

            if (rb != null)
            {
                rb.velocity = new Vector3(0, rb.velocity.y, 0);
            }
        }
    }
}
