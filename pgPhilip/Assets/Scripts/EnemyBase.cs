using UnityEngine;

public abstract class EnemyBase : MonoBehaviour, IHealth
{
    public int health = 1;

    public void TakeDamage()
    {
        health--;
        Debug.Log($"Enemy took damage!");

        if (health <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        Debug.Log("Enemy died!");
        Destroy(gameObject);
    }
}
