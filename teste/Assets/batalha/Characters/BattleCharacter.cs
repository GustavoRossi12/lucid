using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class BattleCharacter : MonoBehaviour
{
    [Header("Data")]
    public CharacterData data;
    public int level = 1;
    public bool isEnemy = false;

    [Header("Runtime Stats")]
    public int currentHP;
    public int maxHP;
    public int currentMP;
    public int maxMP;
    public int attack;
    public int defense;
    public int speed;
    public float critRate;
    public float critDamage;
    public float currentEnergy;
    public float maxEnergy = 100f;

    [Header("State")]
    public bool isAlive = true;
    public bool isTurn = false;
    public List<ActiveStatusEffect> statusEffects = new List<ActiveStatusEffect>();

    [Header("Turn Order")]
    public float actionValue; // lower = acts sooner (HSR style)

    [Header("Events")]
    public UnityEvent<int, bool> onDamageTaken;
    public UnityEvent<int> onHealReceived;
    public UnityEvent onDeath;
    public UnityEvent onTurnStart;
    public UnityEvent onTurnEnd;
    public UnityEvent<float> onEnergyChanged;

    [Header("References")]
    public SpriteRenderer spriteRenderer;
    public Animator animator;
    public Transform vfxSpawnPoint;

    private void Awake()
    {
        if (data != null) InitializeFromData();
    }

    public void InitializeFromData()
    {
        float levelMult = Mathf.Pow(data.hpGrowth, level - 1);
        maxHP = Mathf.RoundToInt(data.baseHP * levelMult);
        maxMP = data.baseMP;
        attack = Mathf.RoundToInt(data.baseAttack * Mathf.Pow(data.attackGrowth, level - 1));
        defense = Mathf.RoundToInt(data.baseDefense * Mathf.Pow(data.defenseGrowth, level - 1));
        speed = Mathf.RoundToInt(data.baseSpeed * Mathf.Pow(data.speedGrowth, level - 1));
        critRate = data.baseCritRate;
        critDamage = data.baseCritDamage;

        currentHP = maxHP;
        currentMP = maxMP;
        currentEnergy = 0;
        isAlive = true;

        // Set action value based on speed (HSR formula-inspired)
        actionValue = 10000f / speed;

        if (spriteRenderer != null && data.battleSprite != null)
            spriteRenderer.sprite = data.battleSprite;
    }

    #region Combat Actions

    public void TakeDamage(int rawDamage, DamageType damageType = DamageType.Physical, bool isCrit = false)
    {
        if (!isAlive) return;

        // Defense reduction
        float defReduction = defense / (defense + 200f + 10f * level);
        int finalDamage = Mathf.Max(1, Mathf.RoundToInt(rawDamage * (1 - defReduction)));

        // Check shields
        finalDamage = ApplyShields(finalDamage);

        currentHP -= finalDamage;
        currentHP = Mathf.Max(0, currentHP);

        // Gain energy from taking damage (HSR mechanic)
        GainEnergy(10f);

        onDamageTaken?.Invoke(finalDamage, isCrit);

        if (animator != null) animator.SetTrigger("Hit");

        if (currentHP <= 0) Die();
    }

    public void Heal(int healAmount)
    {
        if (!isAlive) return;
        int actual = Mathf.Min(healAmount, maxHP - currentHP);
        currentHP += actual;
        onHealReceived?.Invoke(actual);
    }

    public void GainEnergy(float amount)
    {
        currentEnergy = Mathf.Min(maxEnergy, currentEnergy + amount);
        onEnergyChanged?.Invoke(currentEnergy / maxEnergy);
    }

    private int ApplyShields(int damage)
    {
        for (int i = statusEffects.Count - 1; i >= 0; i--)
        {
            var effect = statusEffects[i];
            if (effect.data.effectType == StatusEffectType.Shield)
            {
                int absorbed = Mathf.Min(damage, effect.shieldHP);
                effect.shieldHP -= absorbed;
                damage -= absorbed;
                if (effect.shieldHP <= 0) statusEffects.RemoveAt(i);
                if (damage <= 0) break;
            }
        }
        return damage;
    }

    private void Die()
    {
        isAlive = false;
        if (animator != null) animator.SetTrigger("Die");
        onDeath?.Invoke();
    }

    #endregion

    #region Status Effects

    public void ApplyStatusEffect(StatusEffectData effectData)
    {
        // Check if already has this effect; if so, refresh duration
        var existing = statusEffects.Find(e => e.data == effectData);
        if (existing != null)
        {
            existing.remainingDuration = effectData.duration;
            return;
        }

        var newEffect = new ActiveStatusEffect
        {
            data = effectData,
            remainingDuration = effectData.duration,
            shieldHP = 0
        };
        statusEffects.Add(newEffect);
    }

    public void ProcessStatusEffects()
    {
        List<ActiveStatusEffect> toRemove = new List<ActiveStatusEffect>();
        foreach (var effect in statusEffects)
        {
            // Apply DoT
            if (effect.data.effectType == StatusEffectType.DoT)
            {
                int dotDamage = Mathf.RoundToInt(attack * effect.data.dotMultiplier);
                TakeDamage(dotDamage, effect.data.dotDamageType);
            }

            // Apply regen
            if (effect.data.effectType == StatusEffectType.Regen)
            {
                int healAmt = Mathf.RoundToInt(maxHP * 0.05f);
                Heal(healAmt);
            }

            if (!effect.data.isPermanentUntilBattleEnd)
            {
                effect.remainingDuration--;
                if (effect.remainingDuration <= 0)
                    toRemove.Add(effect);
            }
        }
        foreach (var e in toRemove) statusEffects.Remove(e);
    }

    public bool HasControlEffect()
    {
        return statusEffects.Exists(e => e.data.preventsAction);
    }

    public bool CanUseSkill()
    {
        return !statusEffects.Exists(e => e.data.preventsSkill);
    }

    public bool CanUseUltimate()
    {
        return !statusEffects.Exists(e => e.data.preventsUltimate);
    }

    #endregion

    #region Stat Calculations

    public int GetTotalAttack()
    {
        float multiplier = 1f;
        foreach (var e in statusEffects)
            multiplier += e.data.attackModifier;
        return Mathf.RoundToInt(attack * multiplier);
    }

    public int GetTotalDefense()
    {
        float multiplier = 1f;
        foreach (var e in statusEffects)
            multiplier += e.data.defenseModifier;
        return Mathf.RoundToInt(defense * multiplier);
    }

    public float GetActionValue()
    {
        float spd = speed;
        foreach (var e in statusEffects)
            spd *= (1f + e.data.speedModifier);
        return 10000f / Mathf.Max(1, spd);
    }

    public bool RollCrit()
    {
        float totalCritRate = critRate;
        foreach (var e in statusEffects)
            totalCritRate += e.data.critRateModifier;
        return Random.value < Mathf.Clamp(totalCritRate, 0, 1);
    }

    #endregion

    #region Turn

    public void StartTurn()
    {
        isTurn = true;
        ProcessStatusEffects();
        onTurnStart?.Invoke();
        if (animator != null) animator.SetBool("IsActive", true);
    }

    public void EndTurn()
    {
        isTurn = false;
        // Advance action value for next turn
        actionValue += GetActionValue();
        onTurnEnd?.Invoke();
        if (animator != null) animator.SetBool("IsActive", false);
    }

    #endregion
}

[System.Serializable]
public class ActiveStatusEffect
{
    public StatusEffectData data;
    public int remainingDuration;
    public int shieldHP;
}
