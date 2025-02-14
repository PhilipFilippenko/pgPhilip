using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float damage = 1f;
    public float speed = 50f;
    public float lifetime = 2f;

    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.velocity = transform.forward * speed;
        }

        Destroy(gameObject, lifetime);
    }

    void OnTriggerEnter(Collider other)
    {
        IHealth enemy = other.GetComponent<IHealth>();
        if (enemy != null)
        {
            enemy.TakeDamage();
            Destroy(gameObject);
        }
    }
}
