using UnityEngine;

public class WeaponPickup : MonoBehaviour
{
    public GameObject weaponPrefab;
    internal float rotationSpeed = 50f;
    internal float floatSpeed = 0.5f;
    internal float floatHeight = 0.2f;
    public bool isUsed = false;
    public int remainingAmmo;

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
        if (newY >= startPosition.y + floatHeight) movingUp = false;
        if (newY <= startPosition.y - floatHeight) movingUp = true;

        transform.position = new Vector3(transform.position.x, newY, transform.position.z);
    }

    public void updateUsedStatus(int ammoLeft)
    {
        remainingAmmo = ammoLeft;
        isUsed = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<PlayerController>(out var player))
        {
            player.SetNearbyWeapon(this);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent<PlayerController>(out var player))
        {
            if (player.GetNearbyWeapon() == this)
                player.SetNearbyWeapon(null);
        }
    }

}
