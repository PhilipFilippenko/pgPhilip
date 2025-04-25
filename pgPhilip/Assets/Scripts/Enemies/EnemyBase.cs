using UnityEngine;
using UnityEngine.AI;

namespace Assets.Scripts.Enemies
{
    public abstract class EnemyBase : MonoBehaviour, IHealth
    {
        protected int health = 1;
        protected float attackRange = 2f;
        protected float visionAngle = 1.5f * Mathf.PI;
        protected float memoryTime = 1.5f;
        protected float accelerationTime = 0.3f;
        protected float maxSpeed = 13f;
        protected float rotationSpeed = 15f;

        protected ExitManager exitManager;
        protected PlayerController player;
        protected NavMeshAgent agent;
        protected Animator animator;
        protected WeaponBase currentWeapon;
       
        private Vector3 lastSeenPosition;
        private bool playerVisible;
        private int executionHits = 0;
        private float timeSinceLastSeen = 0f;
        private float reactionTime;
        private float reactionCooldown = 0.2f;
        private float stunTimer = 0f;
        private float nextFireTime = 0f;
        private bool hasPlayedAttackAnim = false;
        private bool isDead = false;
        private bool isStunned = false;
        private bool beingExecuted = false;

        internal bool IsStunned() => isStunned;
        internal bool IsDead() => isDead;

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

            if (currentWeapon != null) currentWeapon.infiniteAmmo = true;
        }

        void Update()
        {
            if (isStunned)
            {
                if (!beingExecuted)
                {
                    stunTimer -= Time.deltaTime;
                    if (stunTimer <= 0f)
                    {
                        isStunned = false;
                        agent.isStopped = false;
                        animator.SetBool("isStunned", false);
                    }
                }
                return;
            }

            reactionTime -= Time.deltaTime;
            UpdateState();
            UpdateAnimatorMovement();

            if (currentState == EnemyState.Attack)
                TryAttackRepeatedly();
        }

        private void UpdateState()
        {
            if (player != null && player.isDead)
            {
                SetState(EnemyState.Idle);
                return;
            }

            playerVisible = CanSeePlayer();

            if (playerVisible)
            {
                if (timeSinceLastSeen > 0f)
                    reactionTime = reactionCooldown;

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
                    HandleIdleState();
                    break;

                case EnemyState.Chase:
                    HandleChaseState();
                    break;

                case EnemyState.Attack:
                    HandleAttackState();
                    break;
            }
        }

        public void Stun()
        {
            if (isStunned) return;

            isStunned = true;
            stunTimer = 3f;
            agent.isStopped = true;

            animator.SetBool("isStunned", true);
            animator.SetFloat("moveX", 0f);
            animator.SetFloat("moveZ", 0f);
        }

        private void HandleIdleState()
        {
            if (playerVisible) SetState(EnemyState.Chase);
        }

        private void HandleChaseState()
        {
            agent.speed = maxSpeed;

            if (!playerVisible && timeSinceLastSeen > memoryTime)
            {
                SetState(EnemyState.Idle);
                return;
            }

            agent.SetDestination(lastSeenPosition);
            SmoothRotateTo(lastSeenPosition);

            float range = currentWeapon != null ? currentWeapon.attackRange : attackRange;
            float distanceSqr = (transform.position - player.transform.position).sqrMagnitude;

            if (distanceSqr < range * range && playerVisible)
                SetState(EnemyState.Attack);
        }

        private void HandleAttackState()
        {
            agent.speed = 0f;

            float range = currentWeapon != null ? currentWeapon.attackRange : attackRange;
            float distanceSqr = (transform.position - player.transform.position).sqrMagnitude;

            if (!playerVisible)
            {
                if (timeSinceLastSeen > memoryTime)
                {
                    SetState(EnemyState.Idle);
                }
                else
                {
                    SetState(EnemyState.Chase);
                }
                return;
            }

            if (distanceSqr > range * range)
            {
                SetState(EnemyState.Chase);
                return;
            }

            if (!hasPlayedAttackAnim)
            {
                OnAttack();
                hasPlayedAttackAnim = true;
            }
        }


        private void TryAttackRepeatedly()
        {
            if (reactionTime > 0f) return;
            if (!playerVisible || currentWeapon == null) return;
            if (currentWeapon.weaponName == "Knife") return;

            if (Time.time >= nextFireTime)
            {
                Vector3 shootDir = (player.transform.position - transform.position).normalized;

                Vector3 lookPos = player.transform.position;
                lookPos.y = transform.position.y;
                transform.LookAt(lookPos);

                if (currentWeapon.ShootAt(shootDir))
                {
                    animator?.Play("RifleShoot", 0, 0f);
                    nextFireTime = Time.time + currentWeapon.fireRate;
                }
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
            if (newState != currentState && newState != EnemyState.Attack)
                hasPlayedAttackAnim = false;

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
                    return hit.collider.CompareTag("Player");
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

        public void StartExecution()
        {
            if (!isStunned || isDead || beingExecuted) return;

            beingExecuted = true;
            isStunned = true;
            stunTimer = 999f;
            agent.isStopped = true;

            animator.SetBool("isStunned", true);
            animator.SetFloat("moveX", 0f);
            animator.SetFloat("moveZ", 0f);
        }


        public void ReceiveExecutionHit()
        {
            if (!beingExecuted || isDead) return;

            executionHits++;
            animator.Play("GetHit", 0, 0f);

            if (executionHits >= 3)
            {
                Die();
            }
        }

        private void Die()
        {
            beingExecuted = false;
            if (isDead) return;
            isDead = true;

            if (agent != null) agent.enabled = false;

            if (!TryGetComponent<Rigidbody>(out var rb))
                rb = gameObject.AddComponent<Rigidbody>();

            rb.constraints = RigidbodyConstraints.FreezeRotation;
            rb.mass = 10f;
            rb.isKinematic = false;

            Vector3 directionFromPlayer = (transform.position - player.transform.position).normalized;
            Vector3 force = directionFromPlayer * 80f + Vector3.up * 10f;
            rb.AddForce(force, ForceMode.Impulse);

            animator?.Play("Death");
            enabled = false;

            OnDeath();

            currentWeapon.gameObject.SetActive(false);

            GameObject floatingPrefab = currentWeapon.GetFloatingWeaponPrefab();
            Vector3 spawnPos = transform.position + Vector3.up * 1f;
            Instantiate(floatingPrefab, spawnPos, Quaternion.identity);

            gameObject.layer = LayerMask.NameToLayer("DeadBody");
            exitManager?.EnemyDefeated();
        }

        protected virtual void OnDeath() { }

        protected virtual void OnAttack() { }
    }
}
