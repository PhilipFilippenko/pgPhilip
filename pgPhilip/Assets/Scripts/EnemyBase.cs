using UnityEngine;

public abstract class EnemyBase : MonoBehaviour, IHealth
{
    internal int health = 1;
    private ExitManager exitManager;
    PlayerController player;
    private Vector3 destination;
  
    float enemyReactionTIme = 0;
    private float enemyReactionTimeCooldown = 0.5f;
    enum EnemyState
    {
        Idle,
        Chase,
        Attack
    }

    EnemyState currentStat = EnemyState.Idle;
    void Start()
    {
        player = FindObjectOfType<PlayerController>();
        exitManager = FindObjectOfType<ExitManager>();
        destination = transform.position;
    }

    public void TakeDamage()
    {
        health--;
        if (health <= 0)
        {
            Die();
        }
    }



    bool canSeePlayer()
    {

        // Check if the player is in line of sight

        // check if the player in visible cone

        // find the angle between the enemy forward vector and the vector pointing to the player

        float angle = Vector3.Angle(transform.forward, player.transform.position - transform.position) ;

        if (Mathf.Abs( angle)  < 45)
        {
            // in the cone of vision

            RaycastHit hit;
            if (Physics.Raycast(transform.position, (player.transform.position - transform.position).normalized, out hit))
            {
                if (hit.collider.CompareTag("Player"))
                {
                    return true;
                }
            }
          

        }
        return false;

    }


    private void Update()
    {

        switch(currentStat)
        {
            case EnemyState.Idle:
                if (canSeePlayer())
                {
                    currentStat = EnemyState.Chase;
                }
                break;
            case EnemyState.Chase:
                if (!canSeePlayer())
                {
                    currentStat = EnemyState.Idle;
                }
                break;
            case EnemyState.Attack:
                break;
        }




        enemyReactionTIme -= Time.deltaTime;
        if (enemyReactionTIme <= 0)
        {
            enemyReactionTIme = enemyReactionTimeCooldown; ;
            if (canSeePlayer())
            {
                destination = player.transform.position;
            }

            // navMeshAgent.destination = destination;
        }

        if (Vector3.Distance(transform.position, destination) > 0.1f)
        {
            
            Vector3 forward = new Vector3(transform.forward.x, 0, transform.forward.z).normalized;
            transform.forward = forward;
            transform.position += forward * Time.deltaTime;
        }
  


    }

    void Die()
    {
        exitManager?.EnemyDefeated();
        Destroy(gameObject);
    }
}
