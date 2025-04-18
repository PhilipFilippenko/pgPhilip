using UnityEngine;

public class Bullet : MonoBehaviour
{
    internal float damage = 1f;
    internal float speed = 100f;
    internal float lifetime = 2f;

    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.velocity = transform.forward * speed;
        Destroy(gameObject, lifetime);
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.TryGetComponent<IHealth>(out IHealth enemy))
        {
            enemy.TakeDamage();
        }
        Destroy(gameObject);
    }
}
