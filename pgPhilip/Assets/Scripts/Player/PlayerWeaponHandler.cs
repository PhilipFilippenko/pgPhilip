using UnityEngine;

public class PlayerWeaponHandler : MonoBehaviour
{
    private PlayerController player;

    public void Initialize(PlayerController p)
    {
        player = p;
    }

    public void EquipWeapon(WeaponBase newWeapon)
    {
        if (newWeapon == null) return;

        if (player.currentWeapon != null)
            Destroy(player.currentWeapon.gameObject);

        player.currentWeapon = newWeapon;

        Transform grip = newWeapon.transform.Find("GripPoint");
        newWeapon.transform.SetParent(player.WeaponHolder, false);
        newWeapon.transform.localPosition = -grip.localPosition;
        newWeapon.transform.localRotation = Quaternion.Inverse(grip.localRotation);
        newWeapon.DisableCollider();
    }

    public void EquipNearbyWeapon()
    {
        WeaponPickup nearby = player.GetNearbyWeapon();
        if (nearby == null) return;

        GameObject instance = Instantiate(nearby.weaponPrefab, player.WeaponHolder.position, player.WeaponHolder.rotation);
        WeaponBase weapon = instance.GetComponent<WeaponBase>();

        if (nearby.isUsed)
            weapon.ammo = nearby.remainingAmmo;

        weapon.SetPickupTemplate(nearby.gameObject);

        EquipWeapon(weapon);
        Destroy(nearby.gameObject);
        player.SetNearbyWeapon(null);
    }

    public void ThrowWeapon()
    {
        if (player.currentWeapon == null) return;

        player.Animator?.Play("MeleeAttack_OneHanded", 0, 0f);

        Transform weaponTransform = player.currentWeapon.transform;
        weaponTransform.SetParent(null);
        weaponTransform.position = player.transform.position + player.transform.forward * 2f;

        Rigidbody rb = weaponTransform.GetComponent<Rigidbody>();
        if (rb == null)
            rb = weaponTransform.gameObject.AddComponent<Rigidbody>();

        rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
        rb.isKinematic = false;
        rb.AddForce(player.transform.forward * 1000f + Vector3.up * 200f);
        rb.AddTorque(Random.insideUnitSphere * 300f);

        player.currentWeapon.Thrown(rb);
        player.currentWeapon.EnableCollider();
        player.currentWeapon = null;
    }
}