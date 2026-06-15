using System.Collections;
using TMPro;
using UnityEngine;

public class UpgradeUI : MonoBehaviour
{
    [Header("Painel")]
    public GameObject panel;
    public RectTransform panelRect;
    public CanvasGroup canvasGroup;
    public float slideDistance = 500f;
    public float slideSpeed = 8f;

    [Header("Textos")]
    public TMP_Text pointsText;
    public TMP_Text speedText;
    public TMP_Text dashText;
    public TMP_Text jumpText;

    private PlayerStats stats;
    private Vector2 targetPos;
    private Vector2 hiddenPos;
    private bool isOpen;

    void Start()
    {
        if (canvasGroup == null) canvasGroup = GetComponent<CanvasGroup>();
        if (panelRect == null) panelRect = GetComponent<RectTransform>();
        if (panelRect == null && panel != null) panelRect = panel.GetComponent<RectTransform>();
        if (panelRect == null) panelRect = GetComponentInChildren<RectTransform>();
        if (panelRect != null)
        {
            targetPos = panelRect.anchoredPosition;
            hiddenPos = targetPos - new Vector2(slideDistance, 0f);
            panelRect.anchoredPosition = hiddenPos;
        }
        if (canvasGroup != null) canvasGroup.alpha = 0f;
    }

    public bool IsOpen => isOpen;

    public void Open(PlayerStats playerStats)
    {
        stats = playerStats;
        isOpen = true;
        Refresh();
        StopAllCoroutines();
        StartCoroutine(SlideIn());
    }

    public void Close()
    {
        isOpen = false;
        StopAllCoroutines();
        StartCoroutine(SlideOut());
    }

    IEnumerator SlideIn()
    {
        if (panel != null) panel.SetActive(true);

        if (panelRect == null)
        {
            if (canvasGroup != null) canvasGroup.alpha = 1f;
            yield break;
        }

        panelRect.anchoredPosition = hiddenPos;
        if (canvasGroup != null) canvasGroup.alpha = 1f;

        while (Vector2.Distance(panelRect.anchoredPosition, targetPos) > 1f)
        {
            panelRect.anchoredPosition = Vector2.Lerp(panelRect.anchoredPosition, targetPos, slideSpeed * Time.deltaTime);
            yield return null;
        }
        panelRect.anchoredPosition = targetPos;
    }

    IEnumerator SlideOut()
    {
        if (panelRect != null)
        {
            while (Vector2.Distance(panelRect.anchoredPosition, hiddenPos) > 1f)
            {
                panelRect.anchoredPosition = Vector2.Lerp(panelRect.anchoredPosition, hiddenPos, slideSpeed * Time.deltaTime);
                yield return null;
            }
            panelRect.anchoredPosition = hiddenPos;
        }

        if (canvasGroup != null) canvasGroup.alpha = 0f;
        if (panel != null) panel.SetActive(false);
    }

    void Refresh()
    {
        if (stats == null) return;
        pointsText.text = $"Pontos: {stats.points}";
        speedText.text = $"Velocidade: Lv.{stats.speedLevel}  (Custo: {(stats.speedLevel + 1) * stats.costPerLevel})";
        dashText.text = $"Dash: Lv.{stats.dashLevel}  (Custo: {(stats.dashLevel + 1) * stats.costPerLevel})";
        jumpText.text = $"Pulo: Lv.{stats.jumpLevel}  (Custo: {(stats.jumpLevel + 1) * stats.costPerLevel})";
    }

    public void BuySpeed() { if (stats != null && stats.UpgradeSpeed()) Refresh(); }
    public void BuyDash() { if (stats != null && stats.UpgradeDash()) Refresh(); }
    public void BuyJump() { if (stats != null && stats.UpgradeJump()) Refresh(); }
}
