using UnityEngine;

public class Bullet : MonoBehaviour
{
    internal float damage = 1f;
    internal float speed = 100f;
    internal float lifetime = 2f;

    private Rigidbody rb;
    internal GameObject owner;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.velocity = transform.forward * speed;
        Destroy(gameObject, lifetime);
    }

    public void Initialize(Vector3 direction, GameObject owner)
    {
        this.owner = owner;
        rb = GetComponent<Rigidbody>();
        rb.velocity = direction * speed;
        Destroy(gameObject, lifetime);
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject == owner) return;

        if (collision.gameObject.TryGetComponent(out IHealth enemy))
        {
            enemy.TakeDamage();
        }
        Destroy(gameObject);
    }
}
