using UnityEngine;

public class Projectile : MonoBehaviour
{
    [Header("Movimento")]
    public float speed = 10f;
    public float homingStrength;
    public float lifetime = 5f;

    [Header("VFX no Player")]
    public GameObject hitOnPlayerVFXPrefab;
    public Vector3 hitOnPlayerVFXAngle;

    [HideInInspector]
    public Transform target;

    void Update()
    {
        if (target != null && homingStrength > 0f)
        {
            Vector3 dir = (target.position - transform.position).normalized;
            Vector3 currentDir = transform.forward;
            Vector3 newDir = Vector3.Lerp(currentDir, dir, homingStrength * Time.deltaTime);
            transform.rotation = Quaternion.LookRotation(newDir);
        }

        transform.position += transform.forward * speed * Time.deltaTime;

        lifetime -= Time.deltaTime;
        if (lifetime <= 0f) Destroy(gameObject);
    }

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        var controller = other.GetComponent<ParkourController>();
        var stats = other.GetComponent<PlayerStats>();
        if (controller != null && controller.EstaDashando && stats != null && stats.isInLifeZone)
        {
            var parry = other.GetComponent<ParryController>();
            if (parry != null) parry.TryTrigger();
        }
        else if (stats != null && stats.isInLifeZone)
        {
            if (stats.HandleDamage())
                UnityEngine.SceneManagement.SceneManager.LoadScene("Morte");
            else if (hitOnPlayerVFXPrefab != null)
                Instantiate(hitOnPlayerVFXPrefab, other.transform.position, Quaternion.Euler(hitOnPlayerVFXAngle));
        }

        Destroy(gameObject);
    }
}
