using UnityEngine;

public class ParryController : MonoBehaviour
{
    [Header("Tempo")]
    public float effectDuration = 5f;
    public float cooldownDuration = 10f;

    [Header("Multiplicadores")]
    [Range(0f, 1f)] public float enemySpeedMultiplier = 0.3f;
    [Range(0f, 1f)] public float attackSpeedMultiplier = 0.5f;

    [Header("Tela Azul")]
    public GameObject bluePanel;

    private bool isActive;
    private float effectTimer;
    private float cooldownTimer;

    public bool IsActive => isActive;
    public float EnemyMul => isActive ? enemySpeedMultiplier : 1f;
    public float AttackMul => isActive ? attackSpeedMultiplier : 1f;

    void Update()
    {
        if (cooldownTimer > 0f)
            cooldownTimer -= Time.deltaTime;

        if (isActive)
        {
            effectTimer -= Time.deltaTime;
            if (effectTimer <= 0f)
            {
                isActive = false;
                if (bluePanel != null) bluePanel.SetActive(false);
            }
        }
    }

    public bool TryTrigger()
    {
        if (isActive || cooldownTimer > 0f) return false;

        isActive = true;
        effectTimer = effectDuration;
        cooldownTimer = cooldownDuration;

        if (bluePanel != null) bluePanel.SetActive(true);
        return true;
    }
}
