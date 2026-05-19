using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Enemy AI that decides actions based on the current battle state.
/// Attach to enemy BattleCharacter GameObjects.
/// </summary>
[RequireComponent(typeof(BattleCharacter))]
public class EnemyAI : MonoBehaviour
{
    public AIBehavior behavior = AIBehavior.Balanced;

    [Header("Aggression Thresholds")]
    [Range(0f, 1f)] public float enragedHPThreshold = 0.3f; // Use powerful skills below this HP

    private BattleCharacter self;

    private void Awake() => self = GetComponent<BattleCharacter>();

    public IEnumerator DecideAndExecute(List<BattleCharacter> playerParty)
    {
        yield return new WaitForSeconds(0.6f); // Thinking delay

        SkillData chosen = ChooseSkill();
        List<BattleCharacter> targets = ChooseTargets(chosen, playerParty);

        if (targets.Count > 0 && chosen != null)
            yield return BattleManager.Instance.ExecuteSkill(self, chosen, targets);
    }

    private SkillData ChooseSkill()
    {
        if (self.data == null) return null;

        float hpRatio = (float)self.currentHP / self.maxHP;
        bool isEnraged = hpRatio <= enragedHPThreshold;

        return behavior switch
        {
            AIBehavior.Aggressive => isEnraged ? (self.data.skill ?? self.data.basicAttack) : self.data.skill ?? self.data.basicAttack,
            AIBehavior.Defensive => isEnraged ? (self.data.skill ?? self.data.basicAttack) : self.data.basicAttack,
            AIBehavior.Balanced => Random.value > 0.4f ? (self.data.skill ?? self.data.basicAttack) : self.data.basicAttack,
            AIBehavior.Randomized => GetRandomSkill(),
            _ => self.data.basicAttack,
        };
    }

    private SkillData GetRandomSkill()
    {
        var skills = new List<SkillData>();
        if (self.data.basicAttack != null) skills.Add(self.data.basicAttack);
        if (self.data.skill != null) skills.Add(self.data.skill);
        return skills.Count > 0 ? skills[Random.Range(0, skills.Count)] : null;
    }

    private List<BattleCharacter> ChooseTargets(SkillData skill, List<BattleCharacter> playerParty)
    {
        if (skill == null) return new List<BattleCharacter>();
        var alive = playerParty.FindAll(c => c.isAlive);
        if (alive.Count == 0) return new List<BattleCharacter>();

        return skill.targetType switch
        {
            TargetType.AllEnemies => alive,
            TargetType.AdjacentEnemies => alive.Count >= 2 ? alive.GetRange(0, 2) : alive,
            TargetType.SingleEnemy => GetPriorityTarget(alive),
            _ => alive.Count > 0 ? new List<BattleCharacter> { alive[0] } : new List<BattleCharacter>(),
        };
    }

    private List<BattleCharacter> GetPriorityTarget(List<BattleCharacter> targets)
    {
        BattleCharacter target = behavior switch
        {
            AIBehavior.Aggressive => targets[Random.Range(0, targets.Count)], // Random for chaos
            AIBehavior.Defensive => targets.Find(t => (float)t.currentHP / t.maxHP < 0.5f) ?? targets[0], // Focus low HP
            _ => targets[0], // Default: front target
        };
        return new List<BattleCharacter> { target };
    }
}

public enum AIBehavior
{
    Aggressive,   // Always uses most powerful skill, focuses on dealing damage
    Defensive,    // More cautious, attacks lowest HP target
    Balanced,     // Mix of basic and skill attacks
    Randomized    // Unpredictable
}
