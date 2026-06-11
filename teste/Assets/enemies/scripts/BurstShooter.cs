using System.Collections;
using UnityEngine;

public class BurstShooter : MonoBehaviour
{
    [Header("Movimento")]
    public float speed = 2f;

    [Header("Disparo")]
    public GameObject projectilePrefab;
    public float projectileSpeed = 8f;
    public float burstDelay = 0.15f;
    public int shotsPerBurst = 3;
    public float burstCooldown = 3f;
    public float attackRange = 10f;

    [Header("Limites")]
    public Transform zoneCenter;
    public float zoneRadius = 6f;

    private Transform player;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        StartCoroutine(BurstRoutine());
    }

    void Update()
    {
        if (player == null) return;

        float dist = Vector3.Distance(transform.position, player.position);

        Vector3 dir = (player.position - transform.position).normalized;
        dir.y = 0f;

        if (dist > attackRange)
        {
            Vector3 newPos = transform.position + dir * speed * Time.deltaTime;
            if (zoneCenter != null)
            {
                Vector3 offset = newPos - zoneCenter.position;
                offset.y = 0f;
                if (offset.magnitude > zoneRadius)
                    offset = offset.normalized * zoneRadius;
                newPos = zoneCenter.position + new Vector3(offset.x, 0f, offset.z);
            }
            transform.position = newPos;
        }

        transform.LookAt(new Vector3(player.position.x, transform.position.y, player.position.z));
    }

    IEnumerator BurstRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(burstCooldown);

            if (player == null) continue;
            float dist = Vector3.Distance(transform.position, player.position);
            if (dist > attackRange) continue;

            for (int i = 0; i < shotsPerBurst; i++)
            {
                if (projectilePrefab != null)
                {
                    var proj = Instantiate(projectilePrefab, transform.position + transform.forward, Quaternion.identity).GetComponent<Projectile>();
                    if (proj != null)
                    {
                        proj.target = player;
                        proj.speed = projectileSpeed;
                        proj.homingStrength = 0f;
                    }
                }
                yield return new WaitForSeconds(burstDelay);
            }
        }
    }
}
