using UnityEngine;

public class PatrolEnemy : MonoBehaviour
{
    [Header("Patrulha")]
    public Transform[] waypoints;
    public float patrolSpeed = 2f;
    public float waitTime = 1f;

    [Header("Perseguição")]
    public float detectionRange = 8f;
    public float chaseSpeed = 4f;

    [Header("Ataque")]
    public float attackRange = 2f;
    public int damage = 1;
    public float attackCooldown = 1.5f;

    private int index;
    private float waitTimer;
    private float attackTimer;
    private Transform player;
    private bool isChasing;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
    }

    void Update()
    {
        if (player == null) return;

        float dist = Vector3.Distance(transform.position, player.position);

        if (dist < detectionRange)
        {
            isChasing = true;
            ChasePlayer(dist);
        }
        else
        {
            isChasing = false;
            Patrol();
        }
    }

    void Patrol()
    {
        if (waypoints == null || waypoints.Length == 0) return;

        Transform target = waypoints[index];
        Vector3 dir = (target.position - transform.position).normalized;
        dir.y = 0f;
        transform.position += dir * patrolSpeed * Time.deltaTime;
        transform.LookAt(new Vector3(target.position.x, transform.position.y, target.position.z));

        if (Vector3.Distance(transform.position, target.position) < 0.5f)
        {
            waitTimer += Time.deltaTime;
            if (waitTimer >= waitTime)
            {
                waitTimer = 0f;
                index = (index + 1) % waypoints.Length;
            }
        }
    }

    void ChasePlayer(float dist)
    {
        Vector3 dir = (player.position - transform.position).normalized;
        dir.y = 0f;
        transform.position += dir * chaseSpeed * Time.deltaTime;
        transform.LookAt(new Vector3(player.position.x, transform.position.y, player.position.z));

        if (dist <= attackRange)
            TryAttack();
    }

    void TryAttack()
    {
        attackTimer -= Time.deltaTime;
        if (attackTimer > 0f) return;
        attackTimer = attackCooldown;

        var stats = player.GetComponent<PlayerStats>();
        if (stats != null && stats.isInLifeZone && stats.HandleDamage())
            UnityEngine.SceneManagement.SceneManager.LoadScene("Morte");
    }
}
