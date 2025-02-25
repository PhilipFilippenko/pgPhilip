using UnityEngine;

public class WeaponPickup : MonoBehaviour
{
    public GameObject weaponPrefab;
    public float rotationSpeed = 50f;
    public float floatSpeed = 0.5f;
    public float floatHeight = 0.2f;

    private Vector3 startPosition;
    private bool movingUp = true;

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
        if (newY >= startPosition.y + floatHeight)
            movingUp = false;
        if (newY <= startPosition.y - floatHeight)
            movingUp = true;

        transform.position = new Vector3(transform.position.x, newY, transform.position.z);
    }

    void OnTriggerEnter(Collider other)
    {
        PlayerController player = other.GetComponent<PlayerController>();
        if (player != null && weaponPrefab != null)

        {
            if (player.currentWeapon != null)
            {
                Destroy(player.currentWeapon.gameObject);
            }

            GameObject newWeaponGO = Instantiate(weaponPrefab, player.weaponHolder.position, player.weaponHolder.rotation);
             WeaponBase newWeapon = newWeaponGO.GetComponent<WeaponBase>();
            string name = "";
            
            if (newWeapon is Rifle)
            {
                name = (newWeapon as Rifle).weaponName;
            }

            Debug.Log("Picked up " + newWeaponGO.name);
            //newWeapon.weaponName = weaponPrefab.weaponName;
            newWeaponGO.transform.SetParent(player.weaponHolder);
            player.EquipWeapon(newWeapon);

            Destroy(gameObject);
        }
    }
}
