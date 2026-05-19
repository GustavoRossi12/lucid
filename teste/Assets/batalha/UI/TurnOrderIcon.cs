using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TurnOrderIcon : MonoBehaviour
{
    public Image portrait;
    public Image background;
    public Image elementBorder;
    public TextMeshProUGUI nameText;
    public GameObject playerIndicator;
    public GameObject enemyIndicator;
    public Animator iconAnimator;

    public void Setup(BattleCharacter character)
    {
        if (character == null) return;

        // Portrait
        if (portrait != null && character.data?.portrait != null)
            portrait.sprite = character.data.portrait;

        // Name
        if (nameText != null)
            nameText.text = character.data != null ? character.data.characterName : "?";

        // Element border color
        if (elementBorder != null && BattleUIManager.Instance != null && character.data != null)
            elementBorder.color = BattleUIManager.Instance.GetElementColor(character.data.element);

        // Player/Enemy indicator
        if (playerIndicator != null) playerIndicator.SetActive(!character.isEnemy);
        if (enemyIndicator != null) enemyIndicator.SetActive(character.isEnemy);

        // Highlight the active character
        if (background != null)
        {
            background.color = character.isTurn
                ? new Color(1f, 0.9f, 0.2f, 0.9f)
                : (character.isEnemy ? new Color(0.8f, 0.3f, 0.3f, 0.7f) : new Color(0.3f, 0.5f, 0.9f, 0.7f));
        }

        if (iconAnimator != null && character.isTurn)
            iconAnimator.SetTrigger("Active");
    }
}
