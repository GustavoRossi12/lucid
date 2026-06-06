using UnityEngine;
using UnityEngine.SceneManagement;

public class EnemyBase : MonoBehaviour
{
    [Header("Movimento")]
    public float speed = 3f;

    [Header("Ataque")]
    public bool isRanged;
    public float attackRange = 2f;
    public float attackCooldown = 2f;

    [Header("Projétil (se ranged)")]
    public GameObject projectilePrefab;
    public float projectileSpeed = 10f;
    public float homingStrength;

    [Header("VFX Ataque")]
    public GameObject attackVFXPrefab;
    public Vector3 attackVFXAngle;

    [Header("VFX Acerto no Player")]
    public GameObject hitOnPlayerVFXPrefab;
    public Vector3 hitOnPlayerVFXAngle;

    [Header("Limites da Zona (Cilindro)")]
    public Transform zoneCenter;
    public float zoneRadius = 5f;
    public float zoneHeight = 5f;

    [Header("Vida")]
    public int maxHealth = 3;
    private int health;

    [Header("Morte")]
    public string deathSceneName = "Morte";
    public GameObject deathVFXPrefab;
    public Vector3 deathVFXAngle;
    public GameObject dropPrefab;

    private Transform player;
    private EnemyVisual visual;
    private float attackTimer;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        visual = GetComponent<EnemyVisual>();
        health = maxHealth;
    }

    void Update()
    {
        if (player == null) return;

        float dist = Vector3.Distance(transform.position, player.position);

        if (isRanged)
        {
            if (dist > attackRange)
                MoveToward(player.position);
            else
                TryAttack();
        }
        else
        {
            if (dist > attackRange)
                MoveToward(player.position);
            else
                TryAttack();
        }
    }

    void MoveToward(Vector3 target)
    {
        Vector3 dir = (target - transform.position).normalized;
        dir.y = 0f;
        float mul = 1f;
        if (player != null)
        {
            var p = player.GetComponent<ParryController>();
            if (p != null) mul = p.EnemyMul;
        }
        Vector3 newPos = transform.position + dir * speed * mul * Time.deltaTime;

        if (zoneCenter != null)
        {
            Vector3 center = zoneCenter.position;
            Vector3 offset = newPos - center;
            offset.y = 0f;
            if (offset.magnitude > zoneRadius)
            {
                offset = offset.normalized * zoneRadius;
                newPos.x = center.x + offset.x;
                newPos.z = center.z + offset.z;
            }
            float halfH = zoneHeight * 0.5f;
            newPos.y = Mathf.Clamp(newPos.y, center.y - halfH, center.y + halfH);
        }

        transform.position = newPos;
    }

    void TryAttack()
    {
        float mul = 1f;
        if (player != null)
        {
            var p = player.GetComponent<ParryController>();
            if (p != null) mul = 1f / Mathf.Max(p.AttackMul, 0.01f);
        }
        attackTimer -= Time.deltaTime * mul;
        if (attackTimer > 0f) return;
        attackTimer = attackCooldown;

        if (isRanged)
            AttackRanged();
        else
            AttackMelee();
    }

    void AttackMelee()
    {
        if (attackVFXPrefab != null)
            Instantiate(attackVFXPrefab, transform.position, Quaternion.Euler(attackVFXAngle));

        if (visual != null) visual.SetAttacking();

        if (player == null) return;

        float dist = Vector3.Distance(transform.position, player.position);
        if (dist > attackRange) return;

        var controller = player.GetComponent<ParkourController>();
        var stats = player.GetComponent<PlayerStats>();
        if (controller != null && controller.EstaDashando && stats != null && stats.isInLifeZone)
        {
            var parry = player.GetComponent<ParryController>();
            if (parry != null) parry.TryTrigger();
            return;
        }

        if (stats != null && stats.isInLifeZone)
        {
            if (stats.HandleDamage())
                SceneManager.LoadScene(deathSceneName);
            else if (hitOnPlayerVFXPrefab != null)
                Instantiate(hitOnPlayerVFXPrefab, player.position, Quaternion.Euler(hitOnPlayerVFXAngle));
        }
    }

    void AttackRanged()
    {
        if (attackVFXPrefab != null)
            Instantiate(attackVFXPrefab, transform.position, Quaternion.Euler(attackVFXAngle));

        if (visual != null) visual.SetAttacking();

        if (projectilePrefab != null && player != null)
        {
            var proj = Instantiate(projectilePrefab, transform.position, Quaternion.identity).GetComponent<Projectile>();
            if (proj != null)
            {
                proj.target = player;
                proj.speed = projectileSpeed;
                proj.homingStrength = homingStrength;
                proj.hitOnPlayerVFXPrefab = hitOnPlayerVFXPrefab;
                proj.hitOnPlayerVFXAngle = hitOnPlayerVFXAngle;
            }
        }
    }

    public void TakeDamage(int amount, Vector3 hitPoint)
    {
        health -= amount;
        if (health <= 0)
        {
            if (deathVFXPrefab != null)
                Instantiate(deathVFXPrefab, transform.position, Quaternion.Euler(deathVFXAngle));

            if (dropPrefab != null)
            {
                Vector3 dropPos = transform.position;
                if (zoneCenter != null)
                {
                    Vector3 offset = dropPos - zoneCenter.position;
                    offset.y = 0f;
                    if (offset.magnitude > zoneRadius)
                        dropPos = zoneCenter.position + offset.normalized * zoneRadius;
                }
                Instantiate(dropPrefab, dropPos, Quaternion.identity);
            }

            Destroy(gameObject);
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = isRanged ? Color.red : Color.yellow;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
