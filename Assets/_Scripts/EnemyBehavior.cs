using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class EnemyBehavior : MonoBehaviour
{
    public enum EnemyState
    {
        Patrolling,
        Chasing,
        Attacking,
        Alerted,
        Dead
    }

    private EnemyState currentState = EnemyState.Patrolling;

    [SerializeField] private LayerMask whatIsGround;
    [SerializeField] private LayerMask whatIsPlayer;
    [SerializeField] private GameObject projectile;
    [SerializeField] private Transform gunPoint;
    
    private NavMeshAgent agent;
    private Transform player;
    private Animator animator;
    private Rigidbody rb;

    [Header("Stats")]
    [Space(10)]
    public float enemyHealth = 50f;

    [SerializeField] private float sightRange;
    [SerializeField] private float attackRange;
    [SerializeField] private float patrolSpeed = 2f;
    [SerializeField] private float chaseSpeed = 4f;
    [SerializeField] private float hitSpeedMultiplier = .35f;
    [SerializeField] private float hitSpeedTime = 1f;

    [Header("Patrolling")]
    [Space(10)]
    //[SerializeField] private float walkPointRange;
    [SerializeField] private bool isCircuit = false;
    [SerializeField] private float closeEnough = 1f;
    [SerializeField] private float stopTime = 2f;
    [SerializeField] private Transform[] patrolPoints;

    private bool isWaiting = false;
    private bool isMovingForward = true;
    private int currentTargetIndex = 0;
    private float stopTimer = 0f;

    private Vector3 walkPoint;
    private bool walkPointSet;

    [Header("Attacking")]
    [Space(10)]
    [SerializeField] private float timeBetweenAttacks;
    [SerializeField] private float forwardShootForce = 32f;
    [SerializeField] private float upwardsShootForce = 2f;
    [Space(10)]

    [Header("FX")]
    [Space(10)]
    [SerializeField] private GameObject hitParticlesPrefab;
    [SerializeField] private GameObject bloodParticlesPrefab;

    private bool hasAttacked;
    private bool isPlayerInSightRange;
    private bool isPlayerInAttackRange;
    private float stateTransitionCooldown = 5f;

    private bool canAttack = true;
    private bool isDead = false;
    private bool isPatrolling = true;
    private bool isChasing = false;
    private float speedMultiplier = 1f;

    public bool IsDead => isDead;

    private void Awake()
    {
        player = FindObjectOfType<PlayerBehavior>().transform;
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponentInChildren<Animator>();
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        SetState();
    }

    void ChangeState(EnemyState newState)
    {
        if (currentState == EnemyState.Dead) return;
        
        currentState = newState;
    }

    private void SetState()
    {
        if (currentState == EnemyState.Dead) return;

        isPlayerInSightRange = Physics.CheckSphere(transform.position, sightRange, whatIsPlayer);
        isPlayerInAttackRange = Physics.CheckSphere(transform.position, attackRange, whatIsPlayer);

        switch (currentState)
        {
            case EnemyState.Patrolling:
                if (isPlayerInSightRange) ChangeState(EnemyState.Chasing);
                Patrolling();
                break;

            case EnemyState.Chasing:
                if (isPlayerInAttackRange) ChangeState(EnemyState.Attacking);
                else if (!isPlayerInSightRange)
                {
                    stateTransitionCooldown -= Time.deltaTime;

                    if (stateTransitionCooldown <= 0f)
                    {
                        ChangeState(EnemyState.Patrolling);
                        stateTransitionCooldown = 5f;
                    }
                }
                else stateTransitionCooldown = 5f;

                Chasing();
                break;

            case EnemyState.Attacking:
                if (!isPlayerInAttackRange) ChangeState(EnemyState.Chasing);
                Attacking();
                break;

            case EnemyState.Alerted:
                if (isPlayerInSightRange) ChangeState(EnemyState.Chasing);
                break;

            case EnemyState.Dead:
                Die();
                break;
        }
    }

    /*private void SetState()
    {
        if (isDead) return;
        
        isPlayerInSightRange = Physics.CheckSphere(transform.position, sightRange, whatIsPlayer);
        isPlayerInAttackRange = Physics.CheckSphere(transform.position, attackRange, whatIsPlayer);

        if (!isPlayerInSightRange && !isPlayerInAttackRange)
        {
            Patrolling();
            UpdateSpeed();
        }

        if (isPlayerInSightRange && !isPlayerInAttackRange)
        {
            Chasing();
            UpdateSpeed();
        }

        if (isPlayerInSightRange && isPlayerInAttackRange) Attacking();
    }*/

    /*private void Patrolling()
    {
        if (!walkPointSet) SearchForWalkPoint();
        else agent.SetDestination(walkPoint);

        Vector3 distanceFromWalkPoint = transform.position - walkPoint;

        if (distanceFromWalkPoint.magnitude < 1f) walkPointSet = false;
    }*/

    /*private void SearchForWalkPoint()
    {
        // how to stop spawn walk points from being set inside of things?
        float randomX = Random.Range(-walkPointRange, walkPointRange);
        float randomZ = Random.Range(-walkPointRange, walkPointRange);

        walkPoint = new Vector3(transform.position.x + randomX, transform.position.y, transform.position.z + randomZ);
        
        if (Physics.Raycast(walkPoint, -transform.up, 2f, whatIsGround)) walkPointSet = true;
    }*/

    private void Patrolling()
    {
        animator.SetBool("isShooting", false);
        animator.SetBool("isChasing", false);
        isPatrolling = true;
        isChasing = false;

        agent.speed = patrolSpeed;
        
        if (!isWaiting)
        {
            agent.SetDestination(patrolPoints[currentTargetIndex].position);
            animator.SetFloat("Speed", agent.velocity.magnitude);

            Vector3 distanceFromTarget = transform.position - patrolPoints[currentTargetIndex].position;

            if (distanceFromTarget.magnitude < closeEnough)
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

            if (stopTimer >= stopTime)
            {
                if (isCircuit)
                {
                    currentTargetIndex++;

                    if (currentTargetIndex >= patrolPoints.Length) currentTargetIndex = 0;
                }
                else
                {
                    if (isMovingForward)
                    {
                        currentTargetIndex++;

                        if (currentTargetIndex == patrolPoints.Length - 1) isMovingForward = false;
                    }
                    else
                    {
                        currentTargetIndex--;

                        if (currentTargetIndex == 0) isMovingForward = true;
                    }
                }

                agent.isStopped = false;
                isWaiting = false;
                animator.SetFloat("Speed", agent.velocity.magnitude);
            }
        }
    }

    private void UpdateAnimation()
    {
        if (!isWaiting && agent.velocity.magnitude > 0.1f) animator.SetFloat("Speed", agent.velocity.magnitude);
        else if (isWaiting) animator.SetFloat("Speed", 0f);
    }

    private void Chasing()
    {
        isPatrolling = false;
        isChasing = true;

        agent.speed = chaseSpeed;
        agent.isStopped = false;
        agent.SetDestination(player.position);
        animator.SetBool("isShooting", false);
        animator.SetBool("isChasing", true);
    }

    private void Attacking()
    {   
        isPatrolling = false;
        isChasing = false;

        animator.SetBool("isChasing", false);
        animator.SetBool("isShooting", true);

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
        // change which projectile enemy is shooting based on the gun it is shooting (different damage)
        Rigidbody projectileRB = Instantiate(projectile, gunPoint.position, Quaternion.identity).GetComponent<Rigidbody>();
        projectileRB.AddForce(transform.forward * forwardShootForce, ForceMode.Impulse);
        //projectileRB.AddForce(transform.up * upwardsShootForce, ForceMode.Impulse);
    }

    private void ResetAttack()
    {
        hasAttacked = false;
    }

    public void TakeDamage(float damage)
    {
        enemyHealth -= damage;
        GameObject hitParticlesObj = Instantiate(hitParticlesPrefab, transform.position, Quaternion.identity);
        Destroy(hitParticlesObj, 2f);

        StartCoroutine(SlowDown());

        if (enemyHealth <= 0)
        {
            ChangeState(EnemyState.Dead);
            Die();
        }

        sightRange *= 3f;
        ChangeState(EnemyState.Alerted);
    }

    private IEnumerator SlowDown()
    {
        speedMultiplier = hitSpeedMultiplier;
        UpdateSpeed();
        yield return new WaitForSeconds(hitSpeedTime);
        speedMultiplier = 1f;
        UpdateSpeed();
    }

    private void UpdateSpeed()
    {
        if (isPatrolling) agent.speed = patrolSpeed * speedMultiplier;
        if (isChasing) agent.speed = chaseSpeed * speedMultiplier;
    }

    private void Die()
    {  
        animator.SetTrigger("Death");
        isDead = true;
        agent.isStopped = true;
        rb.isKinematic = true;
        GetComponent<CapsuleCollider>().enabled = false;
        GameObject bloodParticlesObj = Instantiate(bloodParticlesPrefab, transform.position, Quaternion.identity);
        Destroy(bloodParticlesObj, 5f);
        Destroy(gameObject, 5f);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, sightRange);
    }
}