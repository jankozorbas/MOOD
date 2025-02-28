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
    private Animator animator;

    [Header("Stats")]
    [Space(10)]
    public float enemyHealth = 50f;

    [SerializeField] private float sightRange;
    [SerializeField] private float attackRange;

    [Header("Patrolling")]
    [Space(10)]
    //Dan >>
    [SerializeField] private bool isCircuit = true; 
    [SerializeField] private float stopDuration = 2f;
    [SerializeField] private Transform[] patrolPoints;
    private int currentTargetIndex = 0; 
    private bool isWaiting = false;  
    private float stopTimer = 0f;  

    private bool isMovingForward = true; 
    //Dan <<
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
        animator = GetComponentInChildren<Animator>();  
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

    //Dan >>
    private void Patrolling()
    {
        if (!isWaiting)
        {
            agent.SetDestination(patrolPoints[currentTargetIndex].position);
            animator.SetFloat("Speed", agent.velocity.magnitude);

            Vector3 distanceFromTarget = transform.position - patrolPoints[currentTargetIndex].position;

            if (distanceFromTarget.magnitude < 1f)
            {
                isWaiting = true;
                stopTimer = 0f;  
                agent.isStopped = true;  
                animator.SetFloat("Speed", 0f); 
            }
        }
        else
        {
            stopTimer += Time.deltaTime;

            if (stopTimer >= stopDuration)
            {
                if (isCircuit)
                {
                    currentTargetIndex++;

                    if (currentTargetIndex >= patrolPoints.Length)
                    {
                        currentTargetIndex = 0; 
                    }
                }
                else
                {
                    if (isMovingForward)
                    {
                        currentTargetIndex++;

                        if (currentTargetIndex == patrolPoints.Length - 1)
                        {
                            isMovingForward = false;
                        }
                    }
                    else
                    {
                        currentTargetIndex--;
 
                        if (currentTargetIndex == 0)
                        {
                            isMovingForward = true;  
                        }
                    }
                }

                agent.isStopped = false;
                isWaiting = false;
                animator.SetFloat("Speed", agent.velocity.magnitude);  
            }
        }
    }
    //Dan <<

    private void Chasing()
    {
        agent.SetDestination(player.position);
        animator.SetFloat("Speed", agent.velocity.magnitude); 
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

    //Dan >>
    private void UpdateAnimation()
    {
        
        if (!isWaiting && agent.velocity.magnitude > 0.1f)
        {
            animator.SetFloat("Speed", agent.velocity.magnitude); 
        }
        else if (isWaiting)
        {
            animator.SetFloat("Speed", 0f); 
        }
    }
    //Dan <<

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, sightRange);
    }
}