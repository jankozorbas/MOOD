using UnityEngine;        
using UnityEngine.AI;     

public class EnemyBehavior : MonoBehaviour
{
    [SerializeField] private LayerMask whatIsGround;
    [SerializeField] private LayerMask whatIsPlayer;
    [SerializeField] private GameObject projectile;
    [SerializeField] private Transform gunPoint;

    private NavMeshAgent agent;
    private Transform player;
    private Animator animator;  // Reference to the Animator

    [Header("Stats")]
    [Space(10)]
    public float enemyHealth = 50f;

    [SerializeField] private float sightRange;
    [SerializeField] private float attackRange;

    [Header("Patrolling")]
    [Space(10)]
    [SerializeField] private bool isCircuit = true;  // Toggle between Circuit or Ping-Pong
    [SerializeField] private float stopDuration = 2f;  // Duration to stop at each point
    [SerializeField] private Transform[] patrolPoints; // Array of patrol points (objects in the scene)
    private int currentTargetIndex = 0;  // Current target for the enemy to move to
    private bool isWaiting = false;  // Flag to check if the enemy is waiting at a point
    private float stopTimer = 0f;  // Timer to control the stop duration

    private bool isMovingForward = true;  // Variable to track movement direction (for Ping-Pong behavior)

    [Header("Attacking")]
    [Space(10)]
    [SerializeField] private float timeBetweenAttacks;
    [SerializeField] private float forwardShootForce = 32f;
    [SerializeField] private float upwardsShootForce = 2f;

    private bool hasAttacked;

    private bool isPlayerInSightRange;
    private bool isPlayerInAttackRange;


    private void Awake()
    {
        player = FindObjectOfType<PlayerBehavior>().transform;
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponentInChildren<Animator>();  // Get the Animator component
    }

    private void Update()
    {
        SetState();
        UpdateAnimation();

        
    }

    private void SetState()
    {
        isPlayerInSightRange = Physics.CheckSphere(transform.position, sightRange, whatIsPlayer);
        isPlayerInAttackRange = Physics.CheckSphere(transform.position, attackRange, whatIsPlayer);

        if (!isPlayerInSightRange && !isPlayerInAttackRange) Patrolling();
        if (isPlayerInSightRange && !isPlayerInAttackRange) Chasing();
        if (isPlayerInSightRange && isPlayerInAttackRange) Attacking();
    }

    private void Patrolling()
    {
        if (!isWaiting)
        {
            // Move to the current target point
            agent.SetDestination(patrolPoints[currentTargetIndex].position);
            animator.SetFloat("Speed", agent.velocity.magnitude);

            Vector3 distanceFromTarget = transform.position - patrolPoints[currentTargetIndex].position;

            if (distanceFromTarget.magnitude < 1f)
            {
                // Start waiting after reaching the target
                isWaiting = true;
                stopTimer = 0f;  // Reset the stop timer
                agent.isStopped = true;  // Stop the agent at the target
                animator.SetFloat("Speed", 0f);  // Set the idle animation (speed = 0)
            }
        }
        else
        {
            // Increment the stop timer
            stopTimer += Time.deltaTime;

            // If the timer exceeds the stop duration, move to the next target point
            if (stopTimer >= stopDuration)
            {
                if (isCircuit)
                {
                    // Circuit behavior: Go through points in sequence
                    currentTargetIndex++;

                    if (currentTargetIndex >= patrolPoints.Length)
                    {
                        currentTargetIndex = 0;  // Loop back to the start
                    }
                }
                else
                {
                    // Ping-Pong behavior: Move back and forth between points
                    if (isMovingForward)
                    {
                        currentTargetIndex++;

                        // Check if we've reached the last point
                        if (currentTargetIndex == patrolPoints.Length - 1)
                        {
                            isMovingForward = false;  // Reverse the direction
                        }
                    }
                    else
                    {
                        currentTargetIndex--;

                        // Check if we've reached the first point
                        if (currentTargetIndex == 0)
                        {
                            isMovingForward = true;  // Reverse the direction
                        }
                    }
                }

                // Resume movement and set the next target
                agent.isStopped = false;
                isWaiting = false;
                animator.SetFloat("Speed", agent.velocity.magnitude);  // Resume walking animation
            }
        }
    }

    private void Chasing()
    {
        agent.SetDestination(player.position);
        animator.SetFloat("Speed", agent.velocity.magnitude); // Set chasing animation speed
    }

    private void Attacking()
    {
        agent.SetDestination(transform.position);
        transform.LookAt(player);

        if (!hasAttacked)
        {
            Attack();

            hasAttacked = true;
            Invoke(nameof(ResetAttack), timeBetweenAttacks);
        }
    }

    private void Attack()
    {
        // Change which projectile enemy is shooting based on the gun it is shooting (different damage)
        Rigidbody rb = Instantiate(projectile, gunPoint.position, Quaternion.identity).GetComponent<Rigidbody>();
        rb.AddForce(transform.forward * forwardShootForce, ForceMode.Impulse);
        rb.AddForce(transform.up * upwardsShootForce, ForceMode.Impulse);
    }

    private void ResetAttack()
    {
        hasAttacked = false;
    }

    public void TakeDamage(float damage)
    {
        enemyHealth -= damage;

        if (enemyHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Destroy(gameObject);
    }

    private void UpdateAnimation()
    {
        // This method updates the animation based on the agent's velocity
        if (!isWaiting && agent.velocity.magnitude > 0.1f)
        {
            animator.SetFloat("Speed", agent.velocity.magnitude); // Walking or running
        }
        else if (isWaiting)
        {
            animator.SetFloat("Speed", 0f); // Idle animation while waiting
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, sightRange);
    }
}
