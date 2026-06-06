using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    [Header("Pontos")]
    public int points;

    [Header("Níveis dos Upgrades")]
    public int speedLevel;
    public int dashLevel;
    public int jumpLevel;

    [Header("Custo por nível")]
    public int costPerLevel = 2;

    [Header("Incremento por nível")]
    public float speedIncrease = 1f;
    public float dashIncrease = 3f;
    public float jumpIncrease = 0.5f;

    [Header("Vidas")]
    public int lives;
    public int maxLives = 3;
    public int lifeLevel;
    public int lifeUpgradeCost = 50;

    [Header("Dano")]
    public int damage = 1;
    public int damageLevel;
    public int damageUpgradeCost = 50;

    public bool isInLifeZone;
    private float lastDamageTime;
    public float damageCooldown = 1f;

    private ParkourController controller;

    void Start()
    {
        controller = GetComponent<ParkourController>();
    }

    public void AddPoints(int amount)
    {
        points += amount;
    }

    public bool UpgradeLife()
    {
        if (lifeLevel >= 10) return false;
        if (points < lifeUpgradeCost) return false;
        points -= lifeUpgradeCost;
        lifeLevel++;
        maxLives = 3 + lifeLevel;
        return true;
    }

    public bool UpgradeDamage()
    {
        if (damageLevel >= 10) return false;
        if (points < damageUpgradeCost) return false;
        points -= damageUpgradeCost;
        damageLevel++;
        damage++;
        return true;
    }

    public bool UpgradeSpeed()
    {
        if (speedLevel >= 10) return false;
        int cost = (speedLevel + 1) * costPerLevel;
        if (points < cost) return false;

        points -= cost;
        speedLevel++;
        if (controller != null) controller.maxSpeed += speedIncrease;
        return true;
    }

    public bool UpgradeDash()
    {
        if (dashLevel >= 10) return false;
        int cost = (dashLevel + 1) * costPerLevel;
        if (points < cost) return false;

        points -= cost;
        dashLevel++;
        if (controller != null) controller.dashForce += dashIncrease;
        return true;
    }

    public bool UpgradeJump()
    {
        if (jumpLevel >= 10) return false;
        int cost = (jumpLevel + 1) * costPerLevel;
        if (points < cost) return false;

        points -= cost;
        jumpLevel++;
        if (controller != null) controller.jumpHeight += jumpIncrease;
        return true;
    }

    public bool HandleDamage()
    {
        if (!isInLifeZone || Time.time - lastDamageTime < damageCooldown)
            return false;

        lastDamageTime = Time.time;
        lives--;
        if (lives <= 0)
        {
            lives = 0;
            return true;
        }
        return false;
    }
}
