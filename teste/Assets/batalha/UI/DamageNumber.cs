using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DamageNumber : MonoBehaviour
{
    public TextMeshProUGUI numberText;
    public Animator animator;

    [Header("Colors")]
    public Color normalDamageColor = Color.white;
    public Color critDamageColor = new Color(1f, 0.6f, 0.1f);
    public Color healColor = new Color(0.3f, 1f, 0.4f);
    public Color missColor = new Color(0.7f, 0.7f, 0.7f);

    private Camera mainCamera;

    private void Awake()
    {
        mainCamera = Camera.main;
    }

    public void Show(Vector3 worldPos, int amount, bool isCrit, bool isHeal = false)
    {
        // Position above the character
        if (mainCamera != null)
        {
            Vector3 screenPos = mainCamera.WorldToScreenPoint(worldPos + Vector3.up * 0.5f);
            transform.position = screenPos + new Vector3(Random.Range(-30f, 30f), 0, 0);
        }

        // Format text
        if (numberText != null)
        {
            numberText.text = isHeal ? $"+{amount}" : amount.ToString();
            numberText.color = isHeal ? healColor : (isCrit ? critDamageColor : normalDamageColor);

            if (isCrit)
            {
                numberText.fontSize *= 1.4f;
                numberText.fontStyle = FontStyles.Bold;
            }
        }

        // Animate and destroy
        StartCoroutine(AnimateAndDestroy(isCrit));
    }

    public void ShowMiss()
    {
        if (numberText != null)
        {
            numberText.text = "MISS";
            numberText.color = missColor;
        }
        StartCoroutine(AnimateAndDestroy(false));
    }

    private IEnumerator AnimateAndDestroy(bool isCrit)
    {
        float duration = isCrit ? 1.4f : 1.0f;
        float elapsed = 0f;
        Vector3 startPos = transform.position;
        float riseAmount = isCrit ? 80f : 50f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;

            // Rise and fade
            transform.position = startPos + Vector3.up * (riseAmount * t);

            if (numberText != null)
            {
                Color c = numberText.color;
                c.a = t < 0.6f ? 1f : 1f - ((t - 0.6f) / 0.4f);
                numberText.color = c;
            }

            // Scale punch on crit
            if (isCrit)
            {
                float scalePunch = t < 0.1f ? (1f + t * 3f) : Mathf.Lerp(1.3f, 1f, (t - 0.1f) / 0.3f);
                transform.localScale = Vector3.one * scalePunch;
            }

            yield return null;
        }

        Destroy(gameObject);
    }
}
