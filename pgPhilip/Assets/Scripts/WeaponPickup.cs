using UnityEngine;

public class WeaponPickup : MonoBehaviour
{
    public GameObject weaponPrefab;
    internal float rotationSpeed = 50f;
    internal float floatSpeed = 0.5f;
    internal float floatHeight = 0.2f;
    bool isUsed = false;


    private Vector3 startPosition;
    private bool movingUp = true;
    private int remainingAmmo;

    void Start()
    {
        startPosition = transform.position;
    }

    void Update()
    {
        HandleFloating();
    }

    void HandleFloating()
    {
        transform.Rotate(Vector3.up * rotationSpeed * Time.deltaTime);

        float newY = transform.position.y + (movingUp ? floatSpeed : -floatSpeed) * Time.deltaTime;
        if (newY >= startPosition.y + floatHeight) movingUp = false;
        if (newY <= startPosition.y - floatHeight) movingUp = true;

        transform.position = new Vector3(transform.position.x, newY, transform.position.z);
    }

    void OnTriggerEnter(Collider other)
    {
        PlayerController player = other.GetComponent<PlayerController>();
        if (player == null || weaponPrefab == null) return;

        if (player.currentWeapon != null)
        {
            Destroy(player.currentWeapon.gameObject);
        }

        GameObject newWeaponGO = Instantiate(weaponPrefab, player.weaponHolder.position, player.weaponHolder.rotation);
        WeaponBase newWeapon = newWeaponGO.GetComponent<WeaponBase>();
        if (isUsed)
            newWeapon.ammo = remainingAmmo;

        newWeapon.Iam(gameObject);

        Debug.Log("Picked up " + newWeaponGO.name);

        newWeaponGO.transform.SetParent(player.weaponHolder);
        player.EquipWeapon(newWeapon);

        Destroy(gameObject);
    }

    internal void updateUsedStatus(int ammoLeft)
    {
        remainingAmmo = ammoLeft;
        isUsed = true;
    }

}