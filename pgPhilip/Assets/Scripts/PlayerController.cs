using UnityEngine;

public class PlayerController : MonoBehaviour
{
    float speed = 8f;
    float rotationSpeed = 30f;
    Vector3 movementDirection;
    public WeaponBase currentWeapon;
    public Transform weaponHolder;

    void Update()
    {
        HandleMovement();
        RotateTowardsMouse();
        HandleShooting();
    }

    void HandleMovement()
    {
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveZ = Input.GetAxisRaw("Vertical");

        movementDirection = new Vector3(moveX, 0, moveZ).normalized;

        if (movementDirection.magnitude > 0)
        {
            transform.position += movementDirection * speed * Time.deltaTime;
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
        if (Input.GetMouseButtonDown(0) && currentWeapon != null)
        {
            currentWeapon.Shoot();
        }
    }

    public void EquipWeapon(WeaponBase newWeapon)
    {
        if (newWeapon == null || weaponHolder == null) return;

        if (currentWeapon != null)
        {
            Destroy(currentWeapon.gameObject);
        }

        currentWeapon = Instantiate(newWeapon, weaponHolder.position, weaponHolder.rotation, weaponHolder);
    }

}
