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

    private ParkourController controller;

    void Start()
    {
        controller = GetComponent<ParkourController>();
    }

    public void AddPoints(int amount)
    {
        points += amount;
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
}
