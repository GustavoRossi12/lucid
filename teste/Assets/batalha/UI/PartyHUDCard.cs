using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class PartyHUDCard : MonoBehaviour
{
    [Header("References")]
    public Image portrait;
    public Image hpBar;
    public Image hpBarBack;
    public Image energyBar;
    public Image elementIcon;
    public TextMeshProUGUI characterName;
    public TextMeshProUGUI hpText;
    public Transform statusEffectContainer;
    public GameObject statusEffectIconPrefab;
    public Image activeIndicator;
    public CanvasGroup canvasGroup;

    [Header("Animation")]
    public Animator cardAnimator;

    private BattleCharacter linkedCharacter;
    private float targetHPRatio = 1f;
    private float targetEnergyRatio = 0f;

    public void Initialize(BattleCharacter character)
    {
        linkedCharacter = character;
        if (character.data != null)
        {
            if (portrait != null && character.data.portrait != null)
                portrait.sprite = character.data.portrait;

            if (characterName != null)
                characterName.text = character.data.characterName;

            if (elementIcon != null)
                elementIcon.color = BattleUIManager.Instance
                    ? BattleUIManager.Instance.GetElementColor(character.data.element)
                    : Color.white;
        }

        // Subscribe to events
        character.onDamageTaken.AddListener(OnDamageTaken);
        character.onHealReceived.AddListener(OnHealReceived);
        character.onDeath.AddListener(OnCharacterDeath);
        character.onTurnStart.AddListener(OnTurnStart);
        character.onTurnEnd.AddListener(OnTurnEnd);
        character.onEnergyChanged.AddListener(OnEnergyChanged);

        RefreshUI();
    }

    private void Update()
    {
        // Smooth HP bar animation
        if (hpBar != null)
            hpBar.fillAmount = Mathf.Lerp(hpBar.fillAmount, targetHPRatio, Time.deltaTime * 5f);
        if (energyBar != null)
            energyBar.fillAmount = Mathf.Lerp(energyBar.fillAmount, targetEnergyRatio, Time.deltaTime * 4f);
    }

    private void RefreshUI()
    {
        if (linkedCharacter == null) return;

        targetHPRatio = (float)linkedCharacter.currentHP / linkedCharacter.maxHP;
        targetEnergyRatio = linkedCharacter.currentEnergy / linkedCharacter.maxEnergy;

        if (hpText != null)
            hpText.text = $"{linkedCharacter.currentHP}/{linkedCharacter.maxHP}";

        // Update HP bar color based on HP %
        if (hpBar != null)
        {
            float ratio = targetHPRatio;
            hpBar.color = ratio > 0.5f
                ? Color.Lerp(new Color(1f, 0.7f, 0f), new Color(0.2f, 0.9f, 0.3f), (ratio - 0.5f) * 2f)
                : Color.Lerp(new Color(0.9f, 0.1f, 0.1f), new Color(1f, 0.7f, 0f), ratio * 2f);
        }

        RefreshStatusEffects();
    }

    private void RefreshStatusEffects()
    {
        if (statusEffectContainer == null || statusEffectIconPrefab == null) return;

        // Clear existing icons
        foreach (Transform child in statusEffectContainer)
            Destroy(child.gameObject);

        if (linkedCharacter == null) return;

        foreach (var effect in linkedCharacter.statusEffects)
        {
            var icon = Instantiate(statusEffectIconPrefab, statusEffectContainer);
            var img = icon.GetComponent<Image>();
            if (img != null && effect.data.icon != null)
                img.sprite = effect.data.icon;

            var txt = icon.GetComponentInChildren<TextMeshProUGUI>();
            if (txt != null) txt.text = effect.remainingDuration.ToString();
        }
    }

    #region Event Handlers

    private void OnDamageTaken(int damage, bool isCrit)
    {
        RefreshUI();
        StartCoroutine(HitFlash());

        if (BattleUIManager.Instance != null)
            BattleUIManager.Instance.ShowDamageNumber(linkedCharacter.transform.position + Vector3.up, damage, isCrit);
    }

    private void OnHealReceived(int amount)
    {
        RefreshUI();
        if (BattleUIManager.Instance != null)
            BattleUIManager.Instance.ShowDamageNumber(linkedCharacter.transform.position + Vector3.up, amount, false, true);
    }

    private void OnCharacterDeath()
    {
        if (canvasGroup != null) StartCoroutine(FadeOut());
    }

    private void OnTurnStart()
    {
        if (activeIndicator != null) activeIndicator.gameObject.SetActive(true);
        if (cardAnimator != null) cardAnimator.SetTrigger("Activate");
    }

    private void OnTurnEnd()
    {
        if (activeIndicator != null) activeIndicator.gameObject.SetActive(false);
    }

    private void OnEnergyChanged(float ratio)
    {
        targetEnergyRatio = ratio;
    }

    #endregion

    #region Animations

    private IEnumerator HitFlash()
    {
        if (portrait == null) yield break;
        Color original = portrait.color;
        portrait.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        portrait.color = original;
    }

    private IEnumerator FadeOut()
    {
        float t = 0;
        while (t < 1f)
        {
            t += Time.deltaTime;
            canvasGroup.alpha = 1f - t;
            yield return null;
        }
        canvasGroup.alpha = 0.3f;
        canvasGroup.interactable = false;
    }

    #endregion
}
