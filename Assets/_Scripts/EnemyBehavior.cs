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

    [Header("Stats")]
    [Space(10)]
    public float enemyHealth = 50f;

    [SerializeField] private float sightRange;
    [SerializeField] private float attackRange;

    [Header("Patrolling")]
    [Space(10)]
    [SerializeField] private float walkPointRange;
     
    private Vector3 walkPoint;
    private bool walkPointSet;

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
    }

    private void Update()
    {
        SetState();
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
        if (!walkPointSet) SearchForWalkPoint();
        else agent.SetDestination(walkPoint);

        Vector3 distanceFromWalkPoint = transform.position - walkPoint;

        if (distanceFromWalkPoint.magnitude < 1f) walkPointSet = false;
    }

    private void SearchForWalkPoint()
    {
        // how to stop spawn walk points from being set inside of things?
        float randomX = Random.Range(-walkPointRange, walkPointRange);
        float randomZ = Random.Range(-walkPointRange, walkPointRange);

        walkPoint = new Vector3(transform.position.x + randomX, transform.position.y, transform.position.z + randomZ);
        
        if (Physics.Raycast(walkPoint, -transform.up, 2f, whatIsGround)) walkPointSet = true;
    }

    private void Chasing()
    {
        agent.SetDestination(player.position);
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
        // change which projectile enemy is shooting based on the gun it is shooting (different damage)
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

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, sightRange);
    }
}