using TMPro;
using UnityEngine;

public class UpgradeUI : MonoBehaviour
{
    [Header("Painel")]
    public GameObject panel;

    [Header("Textos")]
    public TMP_Text pointsText;
    public TMP_Text speedText;
    public TMP_Text dashText;
    public TMP_Text jumpText;

    private PlayerStats stats;

    void Start()
    {
        panel.SetActive(false);
    }

    public bool IsOpen => panel.activeSelf;

    public void Open(PlayerStats playerStats)
    {
        stats = playerStats;
        panel.SetActive(true);
        Refresh();
    }

    public void Close()
    {
        panel.SetActive(false);
    }

    void Refresh()
    {
        if (stats == null) return;
        pointsText.text = $"Pontos: {stats.points}";
        speedText.text = $"Velocidade: Lv.{stats.speedLevel}  (Custo: {(stats.speedLevel + 1) * stats.costPerLevel})";
        dashText.text = $"Dash: Lv.{stats.dashLevel}  (Custo: {(stats.dashLevel + 1) * stats.costPerLevel})";
        jumpText.text = $"Pulo: Lv.{stats.jumpLevel}  (Custo: {(stats.jumpLevel + 1) * stats.costPerLevel})";
    }

    public void BuySpeed()
    {
        if (stats != null && stats.UpgradeSpeed())
            Refresh();
    }

    public void BuyDash()
    {
        if (stats != null && stats.UpgradeDash())
            Refresh();
    }

    public void BuyJump()
    {
        if (stats != null && stats.UpgradeJump())
            Refresh();
    }
}
