using UnityEngine;
using UnityEngine.AI;

namespace CustomZombieNamespace
{
    public class CustomZombieCharacterControl : MonoBehaviour
    {
        private enum ControlMode
        {
            Idle,
            Wander,
            FollowPlayer,
            Attack,
            Stunned
        }

        [SerializeField] private float moveSpeed = 2f;
        [SerializeField] private float detectionRange = 10f;
        [SerializeField] private float attackRange = 2f;
        [SerializeField] private float wanderRadius = 10f;
        [SerializeField] private float idleTime = 2f;
        [SerializeField] private float wanderChangeDirectionTime = 5f;
        [SerializeField] private Animator m_animator = null;
        [SerializeField] private int damageAmount = 10;
        [SerializeField] private Collider attackTriggerCollider;
        [SerializeField] private GameObject stunEffectPrefab;

        private Transform player;
        private PlayerHealth playerHealth;
        private NavMeshAgent navMeshAgent;
        private ControlMode currentMode;
        private float wanderTimer;
        private float idleTimer;
        private bool hasDealtDamage = false;

        private bool isStunned = false;
        private float stunTimer;
        private GameObject stunParticlesInstance;

        private void Awake()
        {
            player = GameObject.FindWithTag("Player")?.transform;
            playerHealth = player?.GetComponent<PlayerHealth>();
            navMeshAgent = GetComponent<NavMeshAgent>();
            m_animator = m_animator ?? GetComponent<Animator>();

            if (attackTriggerCollider == null)
            {
                Debug.LogError("Attack trigger collider is not assigned!");
            }
            else
            {
                attackTriggerCollider.enabled = false; 
            }

            if (playerHealth == null)
            {
                Debug.LogError("PlayerHealth component is not found on the player!");
            }
        }

        private void Start()
        {
            navMeshAgent.speed = moveSpeed;
            wanderTimer = wanderChangeDirectionTime;
            idleTimer = idleTime;
            currentMode = ControlMode.Wander;
            SetRandomWanderDestination();
        }

        private void Update()
        {
            if (player == null) return;

            if (isStunned)
            {
                stunTimer -= Time.deltaTime;
                if (stunTimer <= 0f)
                {
                    isStunned = false;
                    currentMode = ControlMode.Wander;
                    navMeshAgent.isStopped = false;
                    Destroy(stunParticlesInstance);
                }
                return;
            }

            float distanceToPlayer = Vector3.Distance(transform.position, player.position);

            if (distanceToPlayer <= detectionRange && currentMode != ControlMode.Attack)
            {
                currentMode = ControlMode.FollowPlayer;
            }

            switch (currentMode)
            {
                case ControlMode.FollowPlayer:
                    FollowPlayer(distanceToPlayer);
                    break;
                case ControlMode.Wander:
                    Wander();
                    break;
                case ControlMode.Attack:
                    HandleAttack(distanceToPlayer);
                    break;
                case ControlMode.Idle:
                    Idle();
                    break;
            }
        }

        private void FollowPlayer(float distanceToPlayer)
        {
            if (distanceToPlayer > detectionRange)
            {
                currentMode = ControlMode.Wander;
                SetRandomWanderDestination();
            }
            else if (distanceToPlayer <= attackRange)
            {
                StartAttack();
            }
            else
            {
                navMeshAgent.speed = moveSpeed;
                navMeshAgent.isStopped = false;
                navMeshAgent.SetDestination(player.position);
                m_animator.SetFloat("MoveSpeed", navMeshAgent.velocity.magnitude);
            }
        }

        private void Wander()
        {
            if (navMeshAgent.remainingDistance <= navMeshAgent.stoppingDistance)
            {
                currentMode = ControlMode.Idle;
                idleTimer = idleTime;
                m_animator.SetFloat("MoveSpeed", 0);
            }
            else
            {
                m_animator.SetFloat("MoveSpeed", navMeshAgent.velocity.magnitude);
            }
        }

        private void Idle()
        {
            idleTimer -= Time.deltaTime;
            if (idleTimer <= 0f)
            {
                currentMode = ControlMode.Wander;
                SetRandomWanderDestination();
            }
            m_animator.SetFloat("MoveSpeed", 0);
        }

        private void SetRandomWanderDestination()
        {
            Vector3 randomDirection = Random.insideUnitSphere * wanderRadius + transform.position;
            NavMeshHit navHit;
            if (NavMesh.SamplePosition(randomDirection, out navHit, wanderRadius, NavMesh.AllAreas))
            {
                navMeshAgent.SetDestination(navHit.position);
                navMeshAgent.isStopped = false;
            }
        }

        private void StartAttack()
        {
            currentMode = ControlMode.Attack;
            navMeshAgent.speed = 0;
            m_animator.SetTrigger("Attack");
            hasDealtDamage = false;
        }

        private void HandleAttack(float distanceToPlayer)
        {
            if (distanceToPlayer > attackRange)
            {
                currentMode = ControlMode.FollowPlayer;
                navMeshAgent.speed = moveSpeed;
                DisableAttackCollider();
                return;
            }

            Vector3 directionToPlayer = player.position - transform.position;
            directionToPlayer.y = 0;
            if (directionToPlayer != Vector3.zero)
            {
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(directionToPlayer), Time.deltaTime * 5f);
            }

            if (attackTriggerCollider.enabled && !hasDealtDamage && playerHealth != null)
            {
                playerHealth.TakeDamage(damageAmount, directionToPlayer);
                hasDealtDamage = true;
            }
        }

        public void EnableAttackCollider()
        {
            if (attackTriggerCollider != null)
            {
                attackTriggerCollider.enabled = true;
                Debug.Log("Collider de atac activat prin Animation Event.");
            }
        }

        public void DisableAttackCollider()
        {
            if (attackTriggerCollider != null)
            {
                attackTriggerCollider.enabled = false;
                Debug.Log("Collider de atac dezactivat prin Animation Event.");
            }
        }

        public void OnAttackEnd()
        {
            currentMode = ControlMode.FollowPlayer;
            navMeshAgent.speed = moveSpeed;
            navMeshAgent.isStopped = false;
            navMeshAgent.SetDestination(player.position);
            DisableAttackCollider();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player") && currentMode == ControlMode.Attack && !hasDealtDamage && playerHealth != null)
            {
                Vector3 hitDirection = player.position - transform.position;
                playerHealth.TakeDamage(damageAmount, hitDirection);
                hasDealtDamage = true;
            }
        }

        public void ApplyStun(float stunDuration)
        {
            isStunned = true;
            stunTimer = stunDuration;
            navMeshAgent.isStopped = true;
            m_animator.SetFloat("MoveSpeed", 0);

            if (stunEffectPrefab != null)
            {
                Vector3 stunPosition = transform.position + Vector3.up * 1.5f;
                stunParticlesInstance = Instantiate(stunEffectPrefab, stunPosition, Quaternion.Euler(-90, 0, 0));
                stunParticlesInstance.transform.SetParent(transform);
            }
        }

        public bool IsStunned => isStunned;

        public void Disappear()
        {
            Destroy(gameObject);
        }
    }
}