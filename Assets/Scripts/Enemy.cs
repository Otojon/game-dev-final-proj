using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    [Header("Stats")]
    public float health = 50f;
    public float attackDamage = 10f;
    public float attackCooldown = 1f;
    private float lastAttackTime = -1f;
    public delegate void EnemyDestroyed();
    public event EnemyDestroyed OnEnemyDestroyed;

    [Header("AI Movement")]
    public float detectionRadius = 15f;
    public Transform player;
    private NavMeshAgent agent;
    private bool isChasing;

    [Header("Animation")]
    private Animation anim;
    public float patrolInterval = 5f;
    public float patrolTimer = 0f;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animation>();

        // Set agent properties for smoother movement
        agent.angularSpeed = 120f; // Adjust for smoother rotation
        agent.acceleration = 8f; // Adjust for smoother acceleration
        agent.stoppingDistance = 2f; // Optional: prevents overshooting
    }

    private void FixedUpdate()
    {
        if (health <= 0) Die();

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (distanceToPlayer <= detectionRadius)
        {
            ChasePlayer();
        }
        else
        {
            Patrol();
        }
    }

    private void Patrol()
    {
        isChasing = false;
        anim.CrossFade("Z_Walk1_InPlace");
        agent.speed = 4;
        patrolTimer += Time.deltaTime;

        if (patrolTimer >= patrolInterval || !agent.hasPath || agent.remainingDistance < 0.5f)
        {
            Vector3 newPosition = RandomNavSphere(transform.position, 1000, -1);
            agent.SetDestination(newPosition);
            patrolTimer = 0f;
        }
    }

    private void ChasePlayer()
    {
        isChasing = true;
        anim.CrossFade("Z_Run_InPlace");
        agent.speed = 10;

        Vector3 directionToPlayer = player.position - transform.position;
        Ray ray = new Ray(transform.position, directionToPlayer);
        if (!Physics.Raycast(ray, directionToPlayer.magnitude))
        {
            agent.SetDestination(player.position);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Player playerScript = other.GetComponent<Player>();
            if (playerScript != null)
            {
                Attack(playerScript);
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Player playerScript = other.GetComponent<Player>();
            if (playerScript != null && Time.time >= lastAttackTime + attackCooldown)
            {
                Attack(playerScript);
            }
        }
    }

    private void Attack(Player player)
    {
        lastAttackTime = Time.time;
        player.TakeDamage(attackDamage);
        anim.CrossFade("Z_Attack");
    }

    public void TakeDamage(float amount)
    {
        health -= amount;
        health = Mathf.Clamp(health, 0, health);
        if (health <= 0) Die();
    }

    private void Die()
    {
        anim.CrossFade("Z_FallingBack");
        OnEnemyDestroyed?.Invoke();
        Destroy(gameObject, 2f);
    }

    public static Vector3 RandomNavSphere(Vector3 origin, float distance, int layerMask)
    {
        Vector3 randomDirection = Random.insideUnitSphere * distance;
        randomDirection += origin;

        NavMeshHit navhit;
        NavMesh.SamplePosition(randomDirection, out navhit, distance, layerMask);
        return navhit.position;
    }
}
