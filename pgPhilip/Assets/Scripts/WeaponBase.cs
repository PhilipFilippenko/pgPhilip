using System;
using UnityEngine;

public abstract class WeaponBase : MonoBehaviour
{
    public string weaponName;
    public int ammo;
    public int maxAmmo;
    public float fireRate;
    public GameObject bulletPrefab;
   // public Transform firePoint;

    protected float nextFireTime = 0f;
    private Collider weaponCollider;
    private bool thrown = false;
    private Rigidbody rb;
    private float timeSinceLastMovement = 1f;
    private float movementThreshold = 0.1f;
    private GameObject pickUPCloneTemplate;

    void Awake()
    {
      
        weaponCollider = GetComponent<Collider>();
    }

    public abstract void Shoot();

    public void DisableCollider()
    {
        if (weaponCollider != null)
        {
            weaponCollider.enabled = false;
        }
    }

    private void Update()
    {
        if (thrown)
        {
            if (rb.velocity.magnitude < movementThreshold)
            {
                timeSinceLastMovement -= Time.deltaTime;
            }
        }

        if (timeSinceLastMovement < 0)
            ChangeTOPickUP();
    }

    private void ChangeTOPickUP()
    {
        pickUPCloneTemplate.SetActive(true);
        pickUPCloneTemplate.transform.position = transform.position;
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
        thrown = true;
        rb = newRB;
    }

    internal void Iam(GameObject gameObject)
    {
      pickUPCloneTemplate = Instantiate<GameObject>(gameObject);
        pickUPCloneTemplate.SetActive(false);
    }
}
