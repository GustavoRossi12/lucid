using UnityEngine;

public class LifeZone : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        if (other.TryGetComponent<PlayerStats>(out var stats))
        {
            if (!stats.isInLifeZone)
                stats.lives = stats.maxLives;
            stats.isInLifeZone = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        if (other.TryGetComponent<PlayerStats>(out var stats))
        {
            stats.isInLifeZone = false;
        }
    }
}
