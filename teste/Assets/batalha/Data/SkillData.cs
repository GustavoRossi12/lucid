using UnityEngine;

[CreateAssetMenu(fileName = "SkillData", menuName = "TurnBattle/Skill Data")]
public class SkillData : ScriptableObject
{
    [Header("Identity")]
    public string skillName = "Attack";
    public string description = "Deal damage to target.";
    public Sprite icon;
    public SkillType skillType = SkillType.BasicAttack;

    [Header("Targeting")]
    public TargetType targetType = TargetType.SingleEnemy;

    [Header("Cost")]
    public int skillPointCost = 0;
    public int energyCost = 0;
    public int energyGain = 20;

    [Header("Damage")]
    public bool dealsDamage = true;
    public float damageMultiplier = 1.0f;
    public DamageType damageType = DamageType.Physical;
    public int hitCount = 1;

    [Header("Healing")]
    public bool heals = false;
    public float healMultiplier = 0f;

    [Header("Effects")]
    public StatusEffectData[] appliedEffects;
    public float effectApplyChance = 1.0f;

    [Header("Animation")]
    public string animationTrigger = "Attack";
    public GameObject vfxPrefab;
    public float animationDuration = 1.5f;
}

public enum SkillType { BasicAttack, Skill, Ultimate, Talent }
public enum TargetType { SingleEnemy, AllEnemies, AdjacentEnemies, SingleAlly, AllAllies, Self }
public enum DamageType { Physical, Fire, Ice, Lightning, Wind }
