using UnityEngine;
using UnityEngine.AI;

namespace CustomPlayerNamespace
{
    public class PlayerAI : MonoBehaviour
    {
        private enum ControlMode
        {
            Wander,
            RunAway
        }

        [SerializeField] private float runSpeed = 3f;
        [SerializeField] private float walkSpeed = 1.5f;
        [SerializeField] private float detectionRange = 10f;
        [SerializeField] private float wanderRadius = 10f;
        [SerializeField] private float changeDirectionTime = 5f;
        [SerializeField] private float wanderTolerance = 1f;
        [SerializeField] private Animator animator = null;

        private Transform zombie;
        private NavMeshAgent navMeshAgent;
        private ControlMode currentMode;
        private float wanderTimer;

        private void Awake()
        {
            zombie = GameObject.FindGameObjectWithTag("ZombiePlayer")?.transform;
            navMeshAgent = GetComponent<NavMeshAgent>();

            if (!navMeshAgent)
            {
                Debug.LogError("NavMeshAgent component missing from the player.");
            }

            if (!animator)
            {
                animator = GetComponent<Animator>();
            }
        }

        private void Start()
        {
            wanderTimer = changeDirectionTime;
            navMeshAgent.speed = walkSpeed;
            currentMode = ControlMode.Wander;
            SetRandomWanderDestination();
        }

        private void Update()
        {
            if (zombie)
            {
                float distanceToZombie = Vector3.Distance(transform.position, zombie.position);

                if (distanceToZombie <= detectionRange)
                {
                    currentMode = ControlMode.RunAway;
                    RunAwayFromZombie();
                }
                else
                {
                    currentMode = ControlMode.Wander;
                    Wander();
                }
            }
        }

        private void Wander()
        {
            wanderTimer -= Time.deltaTime;

            if (wanderTimer <= 0 || navMeshAgent.remainingDistance <= wanderTolerance)
            {
                SetRandomWanderDestination();
                wanderTimer = changeDirectionTime;
            }

            animator.SetFloat("MoveSpeed", walkSpeed);
        }

        private void SetRandomWanderDestination()
        {
            Vector3 randomDirection = Random.insideUnitSphere * wanderRadius;
            randomDirection += transform.position;

            NavMeshHit navHit;

            if (NavMesh.SamplePosition(randomDirection, out navHit, wanderRadius, -1))
            {
                navMeshAgent.SetDestination(navHit.position);
            }
        }

        private void RunAwayFromZombie()
        {
            Vector3 directionAwayFromZombie = (transform.position - zombie.position).normalized;
            Vector3 runPosition = transform.position + directionAwayFromZombie * runSpeed;

            navMeshAgent.speed = runSpeed;
            navMeshAgent.SetDestination(runPosition);

            animator.SetFloat("MoveSpeed", runSpeed);
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (collision.collider.CompareTag("ZombiePlayer"))
            {
                ScoreManager.AddPoints(300);

                ZombieHungerTimer hungerTimer = FindObjectOfType<ZombieHungerTimer>();
                if (hungerTimer != null)
                {
                    hungerTimer.ResetHungerTimer();
                }

                Destroy(gameObject);
            }
        }
    }
}