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
            Debug.Log($"Bullet fired with speed: {rb.velocity} (Expected Speed: {speed})");
        }
        else
        {
            Debug.LogError("Bullet has no Rigidbody!");
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
        else
        {
            Debug.Log($"Bullet hit {collision.gameObject.name}, but it has no IHealth.");
        }

        Destroy(gameObject);
    }
}
