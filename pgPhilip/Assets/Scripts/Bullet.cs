using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float damage = 1f;
    public float speed = 100f;
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



    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.TryGetComponent<IHealth>(out IHealth enemy))
        {
            Debug.Log("Bullet hit an enemy!");
            enemy.TakeDamage();
        }

        Destroy(gameObject);
    }
}
