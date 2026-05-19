using UnityEngine;

[CreateAssetMenu(fileName = "StatusEffect", menuName = "TurnBattle/Status Effect")]
public class StatusEffectData : ScriptableObject
{
    [Header("Identity")]
    public string effectName = "Burn";
    public Sprite icon;
    public Color effectColor = Color.red;
    public StatusEffectType effectType = StatusEffectType.DoT;

    [Header("Duration")]
    public int duration = 2; // turns
    public bool isPermanentUntilBattleEnd = false;

    [Header("DoT Settings")]
    public float dotMultiplier = 0.3f;
    public DamageType dotDamageType = DamageType.Fire;

    [Header("Stat Modifiers")]
    public float attackModifier = 0f;
    public float defenseModifier = 0f;
    public float speedModifier = 0f;
    public float critRateModifier = 0f;

    [Header("Control")]
    public bool preventsAction = false;    // Frozen, Imprisoned
    public bool preventsSkill = false;    // Silence
    public bool preventsUltimate = false;
    public bool forcesRandom = false;     // Confused

    [Header("Visual")]
    public GameObject vfxPrefab;
    public string animationTrigger;
}

public enum StatusEffectType
{
    DoT,          // Burn, Bleed, Shocked
    StatBuff,     // ATK Up, DEF Up
    StatDebuff,   // ATK Down, DEF Down
    Control,      // Frozen, Imprisoned, Confused
    Shield,       // Absorbs damage
    Regen,        // Heal over time
    Special       // Unique effects
}
