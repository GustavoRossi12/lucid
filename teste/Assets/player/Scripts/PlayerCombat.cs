using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    [Header("Combo")]
    public float comboResetTime = 0.5f;
    public float attackDuration = 0.3f;
    public float slamForce = 20f;

    [Header("Attack VFX")]
    public GameObject attack1VFX;
    public Vector3 attack1VFXAngle = Vector3.zero;
    public GameObject attack2VFX;
    public Vector3 attack2VFXAngle = Vector3.zero;
    public GameObject attack3VFX;
    public Vector3 attack3VFXAngle = Vector3.zero;

    [Header("Hit")]
    public GameObject hitVFXPrefab;
    public Vector3 hitVFXAngle = Vector3.zero;
    public float hitRange = 1.5f;
    public float hitRadius = 0.4f;
    public LayerMask hitLayers = -1;

    private ParkourController controller;
    private int comboStep;
    private int airAttackCount;
    private float attackTimer;
    private float resetTimer;
    private bool isAttacking;

    public bool IsAttacking => isAttacking;
    public int CurrentAttack => isAttacking ? comboStep : 0;

    private ParryController parry;

    void Start()
    {
        controller = GetComponent<ParkourController>();
        parry = GetComponent<ParryController>();
    }

    void Update()
    {
        if (isAttacking)
        {
            attackTimer -= Time.deltaTime;
            if (attackTimer <= 0f)
                isAttacking = false;
        }

        if (comboStep > 0)
        {
            resetTimer -= Time.deltaTime;
            if (resetTimer <= 0f)
            {
                comboStep = 0;
                airAttackCount = 0;
            }
        }

        if (Input.GetMouseButtonDown(0))
        {
            PerformAttack();
        }
    }

    void PerformAttack()
    {
        bool grounded = controller.IsGrounded;

        if (grounded)
        {
            airAttackCount = 0;
            comboStep = (comboStep % 3) + 1;
        }
        else
        {
            airAttackCount++;
            comboStep = airAttackCount < 5
                ? (airAttackCount % 2 == 1 ? 1 : 2)
                : 3;

            if (comboStep == 3)
                controller.ApplyKnockback(Vector3.down * slamForce);
        }

        isAttacking = true;
        float mul = parry != null ? parry.AttackMul : 1f;
        attackTimer = attackDuration * mul;
        resetTimer = comboResetTime * mul;

        SpawnAttackVFX();
        CheckHit();
    }

    void SpawnAttackVFX()
    {
        GameObject prefab = null;
        Vector3 angle = Vector3.zero;

        switch (comboStep)
        {
            case 1: prefab = attack1VFX; angle = attack1VFXAngle; break;
            case 2: prefab = attack2VFX; angle = attack2VFXAngle; break;
            case 3: prefab = attack3VFX; angle = attack3VFXAngle; break;
        }

        if (prefab != null)
        {
            GameObject vfx = Instantiate(prefab, transform.position, Quaternion.Euler(angle));
            vfx.transform.SetParent(transform);
        }
    }

    void CheckHit()
    {
        Vector3 center = transform.position + transform.forward * (hitRange * 0.5f) + Vector3.up * 0.2f;
        Collider[] hits = Physics.OverlapSphere(center, hitRadius, hitLayers, QueryTriggerInteraction.Collide);

        foreach (var hit in hits)
        {
            if (hit.transform == transform) continue;

            if (hitVFXPrefab != null)
                Instantiate(hitVFXPrefab, hit.ClosestPoint(transform.position), Quaternion.Euler(hitVFXAngle));

            var enemy = hit.GetComponent<EnemyBase>();
            if (enemy != null)
            {
                var stats = GetComponent<PlayerStats>();
                int dmg = stats != null ? stats.damage : 1;
                enemy.TakeDamage(dmg, hit.ClosestPoint(transform.position));
                break;
            }
        }
    }
}
