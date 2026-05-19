using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using System.Collections;

public class EnemyHUDCard : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    [Header("References")]
    public Image hpBar;
    public Image hpBarBack;
    public Image weaknessBar;        // Toughness bar (HSR mechanic)
    public TextMeshProUGUI enemyName;
    public TextMeshProUGUI hpText;
    public Transform statusEffectContainer;
    public GameObject statusEffectIconPrefab;
    public Image highlightBorder;
    public Image targetArrow;

    [Header("Weakness Icons")]
    public Transform weaknessIconContainer;
    public GameObject weaknessIconPrefab;

    [Header("Toughness")]
    public int maxToughness = 100;
    public int currentToughness;

    private BattleCharacter linkedCharacter;
    private float targetHPRatio = 1f;
    private bool isHighlighted = false;

    public void Initialize(BattleCharacter character)
    {
        linkedCharacter = character;
        currentToughness = maxToughness;

        if (character.data != null)
        {
            if (enemyName != null) enemyName.text = character.data.characterName;
        }

        // Subscribe to events
        character.onDamageTaken.AddListener(OnDamageTaken);
        character.onDeath.AddListener(OnEnemyDeath);

        SetHighlight(false);
        RefreshUI();
    }

    private void Update()
    {
        if (hpBar != null)
            hpBar.fillAmount = Mathf.Lerp(hpBar.fillAmount, targetHPRatio, Time.deltaTime * 6f);
    }

    private void RefreshUI()
    {
        if (linkedCharacter == null) return;
        targetHPRatio = (float)linkedCharacter.currentHP / linkedCharacter.maxHP;

        if (hpText != null)
            hpText.text = $"{linkedCharacter.currentHP}/{linkedCharacter.maxHP}";

        if (weaknessBar != null)
            weaknessBar.fillAmount = (float)currentToughness / maxToughness;

        RefreshStatusEffects();
    }

    public void BreakToughness(int amount)
    {
        currentToughness = Mathf.Max(0, currentToughness - amount);
        if (weaknessBar != null) weaknessBar.fillAmount = (float)currentToughness / maxToughness;

        if (currentToughness == 0)
        {
            // Weakness break! Apply break effect
            StartCoroutine(WeaknessBreakEffect());
        }
    }

    private void RefreshStatusEffects()
    {
        if (statusEffectContainer == null || statusEffectIconPrefab == null) return;
        foreach (Transform child in statusEffectContainer) Destroy(child.gameObject);

        if (linkedCharacter == null) return;
        foreach (var effect in linkedCharacter.statusEffects)
        {
            var icon = Instantiate(statusEffectIconPrefab, statusEffectContainer);
            var img = icon.GetComponent<Image>();
            if (img != null && effect.data.icon != null) img.sprite = effect.data.icon;
            var txt = icon.GetComponentInChildren<TextMeshProUGUI>();
            if (txt != null) txt.text = effect.remainingDuration.ToString();
        }
    }

    public void SetHighlight(bool active)
    {
        isHighlighted = active;
        if (highlightBorder != null)
        {
            highlightBorder.gameObject.SetActive(active);
            highlightBorder.color = active ? new Color(1f, 0.9f, 0f, 0.8f) : Color.clear;
        }
        if (targetArrow != null) targetArrow.gameObject.SetActive(active);
    }

    #region Events

    private void OnDamageTaken(int damage, bool isCrit)
    {
        RefreshUI();
        BreakToughness(isCrit ? 15 : 10);
    }

    private void OnEnemyDeath()
    {
        StartCoroutine(DeathAnimation());
    }

    private IEnumerator DeathAnimation()
    {
        float t = 0;
        var spriteRenderer = linkedCharacter.GetComponent<SpriteRenderer>();
        while (t < 1f)
        {
            t += Time.deltaTime * 1.5f;
            if (spriteRenderer != null)
                spriteRenderer.color = new Color(1, 1, 1, 1f - t);
            yield return null;
        }
        gameObject.SetActive(false);
    }

    private IEnumerator WeaknessBreakEffect()
    {
        if (weaknessBar != null) weaknessBar.color = new Color(1f, 0.4f, 0.1f);
        yield return new WaitForSeconds(0.5f);
        float t = 0;
        while (t < 2f)
        {
            t += Time.deltaTime;
            yield return null;
        }
        // Reset toughness after break
        currentToughness = maxToughness;
        if (weaknessBar != null)
        {
            weaknessBar.color = new Color(0.9f, 0.85f, 0.3f);
            weaknessBar.fillAmount = 1f;
        }
    }

    #endregion

    #region Input

    public void OnPointerClick(PointerEventData eventData)
    {
        if (linkedCharacter != null && linkedCharacter.isAlive)
            BattleUIManager.Instance?.OnTargetClicked(linkedCharacter);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!isHighlighted && linkedCharacter != null && linkedCharacter.isAlive)
        {
            if (highlightBorder != null)
            {
                highlightBorder.gameObject.SetActive(true);
                highlightBorder.color = new Color(1f, 1f, 1f, 0.4f);
            }
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (!isHighlighted)
        {
            if (highlightBorder != null) highlightBorder.gameObject.SetActive(false);
        }
    }

    #endregion
}
