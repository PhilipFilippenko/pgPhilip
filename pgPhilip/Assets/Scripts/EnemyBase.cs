using UnityEngine;
using UnityEngine.AI;

public abstract class EnemyBase : MonoBehaviour, IHealth
{
    internal int health = 1;
    private ExitManager exitManager;
    private PlayerController player;
    private NavMeshAgent agent;

    private float reactionTime;
    private float reactionCooldown = 0.2f;
    private float attackRange = 2f;
    private float visionAngle = 90f;
    private float visionDistance = 15f;

    private enum EnemyState
    {
        Idle,
        Chase,
        Attack
    }

    private EnemyState currentState = EnemyState.Idle;

    void Start()
    {
        player = FindObjectOfType<PlayerController>();
        exitManager = FindObjectOfType<ExitManager>();
        agent = GetComponent<NavMeshAgent>();
        reactionTime = reactionCooldown;
    }

    void Update()
    {
        reactionTime -= Time.deltaTime;

        if (reactionTime <= 0)
        {
            reactionTime = reactionCooldown;
            UpdateState();
        }
    }

    private void UpdateState()
    {
        switch (currentState)
        {
            case EnemyState.Idle:
                if (CanSeePlayer())
                {
                    currentState = EnemyState.Chase;
                }
                break;
            case EnemyState.Chase:
                if (!CanSeePlayer())
                {
                    currentState = EnemyState.Idle;
                }
                else if (Vector3.Distance(transform.position, player.transform.position) < attackRange)
                {
                    currentState = EnemyState.Attack;
                }
                else
                {
                    agent.SetDestination(player.transform.position);
                }
                break;
            case EnemyState.Attack:
                if (!CanSeePlayer())
                {
                    currentState = EnemyState.Idle;
                }
                else if (Vector3.Distance(transform.position, player.transform.position) > attackRange)
                {
                    currentState = EnemyState.Chase;
                }
                else
                {
                    AttackPlayer();
                }
                break;
        }
    }

    private bool CanSeePlayer()
    {
        if (player == null) return false;

        Vector3 directionToPlayer = (player.transform.position - transform.position).normalized;
        float angle = Vector3.Angle(transform.forward, directionToPlayer);

        if (angle < visionAngle / 2f)
        {
            RaycastHit hit;
            if (Physics.Raycast(transform.position + Vector3.up, directionToPlayer, out hit, visionDistance))
            {
                if (hit.collider.CompareTag("Player"))
                {
                    return true;
                }
            }
        }
        return false;
    }

    private void AttackPlayer()
    {
        Debug.Log("Enemy Attacking!");
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
