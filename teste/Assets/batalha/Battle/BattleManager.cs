using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class BattleManager : MonoBehaviour
{
    public static BattleManager Instance { get; private set; }

    [Header("Configuration")]
    public int maxSkillPoints = 5;
    public int startingSkillPoints = 3;

    [Header("Characters")]
    public List<BattleCharacter> playerParty = new List<BattleCharacter>();
    public List<BattleCharacter> enemyParty = new List<BattleCharacter>();

    [Header("State")]
    public BattleState state = BattleState.Idle;
    public BattleCharacter activeCharacter;
    public int currentSkillPoints;
    public int turnCount = 0;

    [Header("Events")]
    public UnityEvent onBattleStart;
    public UnityEvent onBattleEnd;
    public UnityEvent<BattleCharacter> onTurnStart;
    public UnityEvent<BattleResult> onBattleResult;
    public UnityEvent<int> onSkillPointsChanged;
    public UnityEvent<List<BattleCharacter>> onTurnOrderUpdated;

    private List<BattleCharacter> allCharacters = new List<BattleCharacter>();
    private List<BattleCharacter> turnOrder = new List<BattleCharacter>();

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    #region Battle Flow

    public void StartBattle()
    {
        state = BattleState.BattleStart;
        currentSkillPoints = startingSkillPoints;
        turnCount = 0;

        allCharacters.Clear();
        allCharacters.AddRange(playerParty);
        allCharacters.AddRange(enemyParty);

        // Initialize action values
        foreach (var c in allCharacters)
            c.actionValue = c.GetActionValue();

        onBattleStart?.Invoke();
        onSkillPointsChanged?.Invoke(currentSkillPoints);

        StartCoroutine(BattleLoop());
    }

    private IEnumerator BattleLoop()
    {
        yield return new WaitForSeconds(1f);

        while (state != BattleState.BattleEnd)
        {
            // Determine who acts next
            activeCharacter = GetNextCharacter();
            if (activeCharacter == null) break;

            UpdateTurnOrderPreview();

            state = activeCharacter.isEnemy ? BattleState.EnemyTurn : BattleState.PlayerTurn;
            turnCount++;

            activeCharacter.StartTurn();
            onTurnStart?.Invoke(activeCharacter);

            // Wait for turn to complete
            yield return StartCoroutine(ExecuteTurn(activeCharacter));

            activeCharacter.EndTurn();

            // Check battle end
            if (CheckBattleEnd(out BattleResult result))
            {
                state = BattleState.BattleEnd;
                onBattleResult?.Invoke(result);
                onBattleEnd?.Invoke();
                yield break;
            }

            yield return new WaitForSeconds(0.3f);
        }
    }

    private IEnumerator ExecuteTurn(BattleCharacter character)
    {
        if (character.HasControlEffect())
        {
            // Skip turn due to control effect
            yield return new WaitForSeconds(1f);
            yield break;
        }

        if (character.isEnemy)
        {
            yield return StartCoroutine(ExecuteEnemyTurn(character));
        }
        else
        {
            // Player turn - wait for input via UI
            state = BattleState.WaitingForInput;
            while (state == BattleState.WaitingForInput)
                yield return null;
        }
    }

    private IEnumerator ExecuteEnemyTurn(BattleCharacter enemy)
    {
        yield return new WaitForSeconds(0.8f);

        // Simple AI: use skill if available, else basic attack
        SkillData chosenSkill = enemy.data.basicAttack;
        List<BattleCharacter> targets = GetTargets(chosenSkill, enemy, playerParty, enemyParty);

        if (targets.Count > 0)
            yield return StartCoroutine(ExecuteSkill(enemy, chosenSkill, targets));

        yield return new WaitForSeconds(0.5f);
    }

    #endregion

    #region Skill Execution

    public void PlayerUseSkill(SkillData skill, BattleCharacter target)
    {
        if (state != BattleState.WaitingForInput) return;

        // Skill point check
        if (skill.skillType == SkillType.Skill)
        {
            if (currentSkillPoints < skill.skillPointCost) return;
            ModifySkillPoints(-skill.skillPointCost);
        }

        // Basic attack generates skill points
        if (skill.skillType == SkillType.BasicAttack)
            ModifySkillPoints(1);

        // Energy check for ultimate
        if (skill.skillType == SkillType.Ultimate)
        {
            if (activeCharacter.currentEnergy < activeCharacter.maxEnergy) return;
            activeCharacter.currentEnergy = 0;
            activeCharacter.onEnergyChanged?.Invoke(0);
        }

        List<BattleCharacter> targets = GetTargets(skill, activeCharacter, playerParty, enemyParty);
        if (target != null && skill.targetType == TargetType.SingleEnemy)
            targets = new List<BattleCharacter> { target };
        else if (target != null && skill.targetType == TargetType.SingleAlly)
            targets = new List<BattleCharacter> { target };

        StartCoroutine(ExecuteSkillAndContinue(activeCharacter, skill, targets));
    }

    private IEnumerator ExecuteSkillAndContinue(BattleCharacter caster, SkillData skill, List<BattleCharacter> targets)
    {
        state = BattleState.ExecutingAction;
        yield return StartCoroutine(ExecuteSkill(caster, skill, targets));
        state = BattleState.TurnComplete;
    }

    public IEnumerator ExecuteSkill(BattleCharacter caster, SkillData skill, List<BattleCharacter> targets)
    {
        // Animation
        if (caster.animator != null && !string.IsNullOrEmpty(skill.animationTrigger))
            caster.animator.SetTrigger(skill.animationTrigger);

        // VFX
        if (skill.vfxPrefab != null && caster.vfxSpawnPoint != null)
        {
            var vfx = Instantiate(skill.vfxPrefab, caster.vfxSpawnPoint.position, Quaternion.identity);
            Destroy(vfx, 3f);
        }

        yield return new WaitForSeconds(skill.animationDuration * 0.3f);

        // Apply to each target
        foreach (var target in targets)
        {
            if (!target.isAlive) continue;

            // Damage
            if (skill.dealsDamage)
            {
                for (int h = 0; h < skill.hitCount; h++)
                {
                    bool isCrit = caster.RollCrit();
                    float mult = isCrit ? caster.critDamage : 1f;
                    int damage = Mathf.RoundToInt(caster.GetTotalAttack() * skill.damageMultiplier * mult / skill.hitCount);
                    target.TakeDamage(damage, (DamageType)skill.damageType, isCrit);
                    yield return new WaitForSeconds(0.1f);
                }
            }

            // Heal
            if (skill.heals)
            {
                int healAmt = Mathf.RoundToInt(caster.GetTotalAttack() * skill.healMultiplier);
                target.Heal(healAmt);
            }

            // Status effects
            if (skill.appliedEffects != null)
            {
                foreach (var effect in skill.appliedEffects)
                {
                    if (Random.value <= skill.effectApplyChance)
                        target.ApplyStatusEffect(effect);
                }
            }
        }

        // Energy gain
        caster.GainEnergy(skill.energyGain);

        yield return new WaitForSeconds(skill.animationDuration * 0.7f);
    }

    #endregion

    #region Turn Order (HSR-Style)

    private BattleCharacter GetNextCharacter()
    {
        var alive = allCharacters.Where(c => c.isAlive).ToList();
        if (alive.Count == 0) return null;

        // Find character with minimum action value
        BattleCharacter next = alive.OrderBy(c => c.actionValue).First();

        // Advance all action values (subtract the minimum, simulating time passing)
        float minAV = next.actionValue;
        foreach (var c in alive)
            c.actionValue -= minAV;

        return next;
    }

    public void UpdateTurnOrderPreview()
    {
        // Simulate next N turns without modifying real state
        var simChars = allCharacters
            .Where(c => c.isAlive)
            .Select(c => new { character = c, av = c.actionValue })
            .ToList();

        List<BattleCharacter> preview = new List<BattleCharacter>();
        var avCopy = simChars.ToDictionary(x => x.character, x => x.av);

        for (int i = 0; i < 8; i++)
        {
            if (avCopy.Count == 0) break;
            var next = avCopy.OrderBy(kv => kv.Value).First();
            preview.Add(next.Key);
            float baseAV = next.Key.GetActionValue();
            avCopy[next.Key] += baseAV;
        }

        onTurnOrderUpdated?.Invoke(preview);
    }

    #endregion

    #region Utilities

    private List<BattleCharacter> GetTargets(SkillData skill, BattleCharacter caster,
        List<BattleCharacter> allies, List<BattleCharacter> enemies)
    {
        bool casterIsPlayer = !caster.isEnemy;
        var aliveEnemies = enemies.Where(c => c.isAlive).ToList();
        var aliveAllies = allies.Where(c => c.isAlive).ToList();
        var playerAlive = playerParty.Where(c => c.isAlive).ToList();

        switch (skill.targetType)
        {
            case TargetType.SingleEnemy:
            {
                var list = casterIsPlayer ? aliveEnemies : playerAlive;
                return list.Count > 0 ? new List<BattleCharacter> { list[0] } : new List<BattleCharacter>();
            }
            case TargetType.AllEnemies:
                return casterIsPlayer ? aliveEnemies : playerAlive;
            case TargetType.AdjacentEnemies:
            {
                var list = casterIsPlayer ? aliveEnemies : playerAlive;
                return list.Take(2).ToList();
            }
            case TargetType.SingleAlly:
                return new List<BattleCharacter> { caster };
            case TargetType.AllAllies:
                return casterIsPlayer ? aliveAllies : enemyParty.Where(c => c.isAlive).ToList();
            case TargetType.Self:
                return new List<BattleCharacter> { caster };
            default:
                return new List<BattleCharacter>();
        }
    }

    public void ModifySkillPoints(int delta)
    {
        currentSkillPoints = Mathf.Clamp(currentSkillPoints + delta, 0, maxSkillPoints);
        onSkillPointsChanged?.Invoke(currentSkillPoints);
    }

    private bool CheckBattleEnd(out BattleResult result)
    {
        bool allEnemiesDead = enemyParty.All(c => !c.isAlive);
        bool allPlayersDead = playerParty.All(c => !c.isAlive);

        if (allEnemiesDead) { result = BattleResult.Victory; return true; }
        if (allPlayersDead) { result = BattleResult.Defeat; return true; }
        result = BattleResult.Ongoing;
        return false;
    }

    public void SignalTurnComplete()
    {
        if (state == BattleState.WaitingForInput)
            state = BattleState.TurnComplete;
    }

    #endregion
}

public enum BattleState
{
    Idle, BattleStart, PlayerTurn, EnemyTurn,
    WaitingForInput, ExecutingAction, TurnComplete, BattleEnd
}

public enum BattleResult { Ongoing, Victory, Defeat }
