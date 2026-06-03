using UnityEngine;
using UnityEngine.SceneManagement;

public class EnemyChaser : MonoBehaviour
{
    [Header("Movimento")]
    public float baseSpeed = 3f;
    public float speedMultiplier = 0.5f;
    public float stopDistance = 1f;

    [Header("Morte")]
    public string deathSceneName = "Morte";

    [Header("Sprite")]
    public SpriteRenderer sprite;

    private Transform player;
    private bool isActive;
    private bool isDead;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        if (sprite != null) sprite.enabled = isActive;
    }

    void Update()
    {
        if (player == null || !isActive || isDead) return;

        Vector3 flatPos = transform.position;
        flatPos.y = 0;
        Vector3 flatPlayer = player.position;
        flatPlayer.y = 0;

        float distance = Vector3.Distance(flatPos, flatPlayer);

        if (distance <= stopDistance)
        {
            isDead = true;
            SceneManager.LoadScene(deathSceneName);
            return;
        }

        float currentSpeed = baseSpeed + distance * speedMultiplier;

        Vector3 dir = (player.position - transform.position).normalized;
        dir.y = 0;
        transform.position += dir * currentSpeed * Time.deltaTime;

        if (Camera.main != null)
        {
            Vector3 look = Camera.main.transform.position;
            look.y = transform.position.y;
            transform.LookAt(look);
        }
    }

    public void Activate()
    {
        isActive = true;
        if (sprite != null) sprite.enabled = true;
    }
}
