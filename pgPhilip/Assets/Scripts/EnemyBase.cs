using UnityEngine;

public abstract class EnemyBase : MonoBehaviour, IHealth
{
    public int health = 1;

    public void TakeDamage()
    {
        health--;
        if (health <= 0)
        {
            Destroy(gameObject);
        }
    }
}
