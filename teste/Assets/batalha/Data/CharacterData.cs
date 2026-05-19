using UnityEngine;

[CreateAssetMenu(fileName = "CharacterData", menuName = "TurnBattle/Character Data")]
public class CharacterData : ScriptableObject
{
    [Header("Identity")]
    public string characterName = "Hero";
    public Sprite portrait;
    public Sprite battleSprite;
    public Color elementColor = Color.white;
    public ElementType element = ElementType.None;

    [Header("Base Stats")]
    public int baseHP = 1000;
    public int baseMP = 100;
    public int baseAttack = 100;
    public int baseDefense = 50;
    public int baseSpeed = 100;
    public float baseCritRate = 0.05f;
    public float baseCritDamage = 1.5f;

    [Header("Skills")]
    public SkillData basicAttack;
    public SkillData skill;
    public SkillData ultimate;
    public SkillData talent;

    [Header("Leveling")]
    public float hpGrowth = 1.1f;
    public float attackGrowth = 1.08f;
    public float defenseGrowth = 1.05f;
    public float speedGrowth = 1.02f;
}

public enum ElementType { None, Fire, Ice, Lightning, Wind, Physical }
