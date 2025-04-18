using UnityEngine;

public class Knife : WeaponBase
{
    private Vector3 gizmoPosition;
    private float gizmoTimer = 0f;
    private float gizmoDuration = 0.5f;
    private float gizmoRadius = 1f;

    void Awake()
    {
        weaponName = "Knife";
        ammo = 0;
        maxAmmo = 0;
        fireRate = 0f;
        attackRange = 1.7f;
        isMelee = true;
    }

    public override bool Attack()
    {
        gizmoPosition = transform.root.position + transform.root.forward * attackRange * 0.5f;
        gizmoTimer = gizmoDuration;

        return TryAttack(gizmoPosition, gizmoRadius);
    }
}