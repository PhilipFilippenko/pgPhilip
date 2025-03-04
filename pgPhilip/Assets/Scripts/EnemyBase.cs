using UnityEngine;

public abstract class EnemyBase : MonoBehaviour, IHealth
{
    internal int health = 1;
    private ExitManager exitManager;

    void Start()
    {
        exitManager = FindObjectOfType<ExitManager>();
    }

    public void TakeDamage()
    {
        health--;
        if (health <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        exitManager?.EnemyDefeated();
        Destroy(gameObject);
    }
}
