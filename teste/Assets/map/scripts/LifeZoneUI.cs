using TMPro;
using UnityEngine;

public class LifeZoneUI : MonoBehaviour
{
    [Header("UI")]
    public GameObject panel;
    public TMP_Text heartsText;

    [Header("Símbolos")]
    public string heartFilled = "<color=red>♥</color>";
    public string heartEmpty = "<color=gray>♥</color>";

    private PlayerStats stats;

    void Start()
    {
        if (panel != null) panel.SetActive(false);
    }

    void Update()
    {
        if (stats == null)
            stats = FindAnyObjectByType<PlayerStats>();

        if (stats == null) return;

        if (stats.isInLifeZone)
        {
            if (panel != null) panel.SetActive(true);
            UpdateHearts();
        }
        else
        {
            if (panel != null) panel.SetActive(false);
        }
    }

    void UpdateHearts()
    {
        if (heartsText == null) return;

        string text = "";
        for (int i = 0; i < stats.maxLives; i++)
            text += i < stats.lives ? heartFilled : heartEmpty;

        heartsText.text = text;
    }
}
