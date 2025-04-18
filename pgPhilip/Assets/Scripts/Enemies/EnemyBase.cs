using UnityEngine;
using UnityEngine.AI;

namespace Assets.Scripts.Enemies
{
    public abstract class EnemyBase : MonoBehaviour, IHealth
    {
        private int health = 1;
        private float attackRange = 2f;
        private float visionAngle = 1.5f * Mathf.PI;
        private float memoryTime = 1f;
        private float accelerationTime = 0.3f;
        private float maxSpeed = 13f;
        private float rotationSpeed = 15f;

        private ExitManager exitManager;
        private PlayerController player;
        private NavMeshAgent agent;
        private Animator animator;

        private Vector3 lastSeenPosition;
        private bool playerVisible;
        private float timeSinceLastSeen = 0f;
        private float reactionTime;
        private float reactionCooldown = 0.2f;

        private WeaponBase currentWeapon;

        private enum EnemyState { Idle, Chase, Attack }
        private EnemyState currentState = EnemyState.Idle;

        void Start()
        {
            player = FindObjectOfType<PlayerController>();
            exitManager = FindObjectOfType<ExitManager>();
            agent = GetComponent<NavMeshAgent>();
            currentWeapon = GetComponentInChildren<WeaponBase>();

            animator = GetComponentInChildren<Animator>();

            agent.updateRotation = false;
            agent.acceleration = maxSpeed / accelerationTime;

            reactionTime = reactionCooldown;
        }

        void Update()
        {
            reactionTime -= Time.deltaTime;
            UpdateState();

            UpdateAnimatorMovement();
        }

        private void UpdateState()
        {
            playerVisible = CanSeePlayer();

            if (playerVisible)
            {
                lastSeenPosition = player.transform.position;
                timeSinceLastSeen = 0f;
            }
            else
            {
                timeSinceLastSeen += Time.deltaTime;
            }

            switch (currentState)
            {
                case EnemyState.Idle:
                    agent.speed = 2f;
                    if (playerVisible)
                    {
                        SetState(EnemyState.Chase);
                    }
                    break;

                case EnemyState.Chase:
                    agent.speed = maxSpeed;

                    if (!playerVisible && timeSinceLastSeen > memoryTime)
                    {
                        SetState(EnemyState.Idle);
                    }
                    else
                    {
                        agent.SetDestination(lastSeenPosition);
                        SmoothRotateTo(lastSeenPosition);

                        float distanceSqr = (transform.position - player.transform.position).sqrMagnitude;

                        if (distanceSqr < attackRange * attackRange)
                        {
                            SetState(EnemyState.Attack);
                        }
                    }
                    break;

                case EnemyState.Attack:
                    agent.speed = 0f;

                    if (!playerVisible && timeSinceLastSeen > memoryTime)
                    {
                        SetState(EnemyState.Idle);
                    }
                    else if ((transform.position - player.transform.position).sqrMagnitude > attackRange * attackRange)
                    {
                        SetState(EnemyState.Chase);
                    }
                    else
                    {
                        animator.Play("MeleeAttack_OneHanded", 0, 0f);
                        currentWeapon?.Attack();
                    }
                    break;
            }
        }

        private void UpdateAnimatorMovement()
        {
            if (agent == null || animator == null) return;

            Vector3 localDir = transform.InverseTransformDirection(agent.velocity.normalized);

            animator.SetFloat("moveX", localDir.x);
            animator.SetFloat("moveZ", localDir.z);
        }

        private void SetState(EnemyState newState)
        {
            currentState = newState;
        }

        private void SmoothRotateTo(Vector3 targetPosition)
        {
            Vector3 direction = (targetPosition - transform.position).normalized;
            if (direction.sqrMagnitude > 0.01f)
            {
                Quaternion targetRotation = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            }
        }

        private bool CanSeePlayer()
        {
            if (player == null) return false;

            Vector3 directionToPlayer = removeY(player.transform.position - transform.position);
            float angle = Mathf.Acos(Vector3.Dot(removeY(transform.forward), directionToPlayer));

            if (angle < (visionAngle / 2f))
            {
                if (Physics.Raycast(transform.position, directionToPlayer, out RaycastHit hit))
                {
                    return hit.collider.CompareTag("Player");
                }
            }
            return false;
        }

        private Vector3 removeY(Vector3 v)
        {
            return new Vector3(v.x, 0, v.z).normalized;
        }

        public void TakeDamage()
        {
            health--;
            if (health <= 0) Die();
        }

        private void Die()
        {
            OnDeath();
            exitManager?.EnemyDefeated();
            Destroy(gameObject);
        }

        protected virtual void OnDeath() { }

        public void EquipWeapon(WeaponBase weapon)
        {
            currentWeapon = weapon;
        }
    }
}
