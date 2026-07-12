using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeUI : MonoBehaviour
{
    [Header("Painel")]
    public GameObject panel;
    public RectTransform panelRect;
    public CanvasGroup canvasGroup;
    public float slideDistance = 500f;
    public float slideSpeed = 8f;

    [Header("UI")]
    public TMP_Text pointsText;
    public TMP_Text costText;
    public TMP_Text resultName;
    public Image resultIcon;

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
        if (resultIcon != null) resultIcon.gameObject.SetActive(false);
        if (resultName != null) resultName.gameObject.SetActive(false);
    }

    public bool IsOpen => isOpen;

    public void Open(PlayerStats playerStats)
    {
        stats = playerStats;
        isOpen = true;
        Refresh();
        gameObject.SetActive(true);
        StopAllCoroutines();
        StartCoroutine(SlideIn());
    }

    public void Close()
    {
        isOpen = false;
        if (!gameObject.activeInHierarchy)
        {
            CompleteSlideOut();
            return;
        }
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

    void CompleteSlideOut()
    {
        if (panelRect != null) panelRect.anchoredPosition = hiddenPos;
        if (canvasGroup != null) canvasGroup.alpha = 0f;
        if (panel != null) panel.SetActive(false);
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
        if (costText != null) costText.text = $"Custo: {stats.rollCost}";
    }

    public void Roll()
    {
        if (stats == null) return;

        var item = stats.RollCollectible();
        if (item == null) return;

        Refresh();

        if (resultIcon != null)
        {
            resultIcon.sprite = item.icon;
            resultIcon.gameObject.SetActive(true);
        }
        if (resultName != null)
        {
            resultName.text = item.itemName;
            resultName.color = CollectibleData.GetRarityColor(item.rarity);
            resultName.gameObject.SetActive(true);
        }
    }
}
