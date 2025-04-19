namespace Assets.Scripts.Enemies
{
    public class EnemyGuard : EnemyBase
    {
        protected override void OnAttack()
        {
            if (currentWeapon == null) return;

            string name = currentWeapon.weaponName;

            if (name == "Knife")
            {
                animator.Play("MeleeAttack_OneHanded", 0, 0f);
                currentWeapon.Attack();
            }
            else
            {
                currentWeapon.Shoot();
            }
        }
    }
}