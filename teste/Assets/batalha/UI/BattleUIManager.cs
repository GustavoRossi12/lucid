using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BattleUIManager : MonoBehaviour
{
    public static BattleUIManager Instance { get; private set; }

    [Header("Action Bar")]
    public Transform turnOrderContainer;
    public GameObject turnOrderIconPrefab;
    private List<GameObject> turnOrderIcons = new List<GameObject>();

    [Header("Skill Buttons")]
    public Button btnBasicAttack;
    public Button btnSkill;
    public Button btnUltimate;
    public Image ultimateReadyGlow;
    public TextMeshProUGUI skillPointText;

    [Header("Skill Point Pips")]
    public Transform skillPointContainer;
    public GameObject skillPointPipPrefab;
    private List<GameObject> skillPips = new List<GameObject>();

    [Header("Party HUD")]
    public Transform partyHUDContainer;
    public GameObject partyHUDCardPrefab;
    private Dictionary<BattleCharacter, PartyHUDCard> partyCards = new Dictionary<BattleCharacter, PartyHUDCard>();

    [Header("Enemy HUD")]
    public Transform enemyHUDContainer;
    public GameObject enemyHUDCardPrefab;
    private Dictionary<BattleCharacter, EnemyHUDCard> enemyCards = new Dictionary<BattleCharacter, EnemyHUDCard>();

    [Header("Target Selection")]
    public GameObject targetArrowPrefab;
    private BattleCharacter selectedTarget;
    private bool isSelectingTarget = false;
    private System.Action<BattleCharacter> onTargetSelected;

    [Header("Damage Numbers")]
    public GameObject damageNumberPrefab;
    public Canvas worldCanvas;

    [Header("Panels")]
    public GameObject victoryPanel;
    public GameObject defeatPanel;
    public GameObject turnIndicatorPanel;
    public TextMeshProUGUI turnIndicatorText;
    public TextMeshProUGUI turnCountText;

    [Header("Element Colors")]
    public Color fireColor = new Color(1f, 0.4f, 0.1f);
    public Color iceColor = new Color(0.4f, 0.8f, 1f);
    public Color lightningColor = new Color(0.9f, 0.7f, 1f);
    public Color windColor = new Color(0.3f, 0.9f, 0.5f);
    public Color physicalColor = new Color(0.7f, 0.7f, 0.7f);

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        SetupButtonListeners();
        HideActionButtons();
        HideResultPanels();
    }

    #region Setup

    private void SetupButtonListeners()
    {
        btnBasicAttack.onClick.AddListener(OnBasicAttackClicked);
        btnSkill.onClick.AddListener(OnSkillClicked);
        btnUltimate.onClick.AddListener(OnUltimateClicked);
    }

    public void InitializeBattle(List<BattleCharacter> party, List<BattleCharacter> enemies)
    {
        // Create party HUD cards
        foreach (var member in party)
        {
            if (partyHUDCardPrefab != null && partyHUDContainer != null)
            {
                var card = Instantiate(partyHUDCardPrefab, partyHUDContainer).GetComponent<PartyHUDCard>();
                if (card != null)
                {
                    card.Initialize(member);
                    partyCards[member] = card;
                }
            }
        }

        // Create enemy HUD cards
        foreach (var enemy in enemies)
        {
            if (enemyHUDCardPrefab != null && enemyHUDContainer != null)
            {
                var card = Instantiate(enemyHUDCardPrefab, enemyHUDContainer).GetComponent<EnemyHUDCard>();
                if (card != null)
                {
                    card.Initialize(enemy);
                    enemyCards[enemy] = card;
                }
            }
        }

        // Create skill point pips
        CreateSkillPointPips(BattleManager.Instance.maxSkillPoints);
    }

    private void CreateSkillPointPips(int max)
    {
        foreach (var pip in skillPips) Destroy(pip);
        skillPips.Clear();

        for (int i = 0; i < max; i++)
        {
            if (skillPointPipPrefab != null && skillPointContainer != null)
            {
                var pip = Instantiate(skillPointPipPrefab, skillPointContainer);
                skillPips.Add(pip);
            }
        }
    }

    #endregion

    #region Turn Management

    public void OnPlayerTurnStart(BattleCharacter character)
    {
        ShowActionButtons(character);
        ShowTurnIndicator("YOUR TURN", Color.white);
        RefreshUltimateGlow(character);
    }

    public void OnEnemyTurnStart(BattleCharacter enemy)
    {
        HideActionButtons();
        ShowTurnIndicator(enemy.data ? enemy.data.characterName : "Enemy", Color.red);
    }

    private void ShowTurnIndicator(string text, Color color)
    {
        if (turnIndicatorPanel != null)
        {
            turnIndicatorPanel.SetActive(true);
            if (turnIndicatorText != null)
            {
                turnIndicatorText.text = text;
                turnIndicatorText.color = color;
            }
            StartCoroutine(HideTurnIndicatorAfterDelay(1.2f));
        }
    }

    private IEnumerator HideTurnIndicatorAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (turnIndicatorPanel != null) turnIndicatorPanel.SetActive(false);
    }

    #endregion

    #region Turn Order

    public void UpdateTurnOrder(List<BattleCharacter> order)
    {
        // Clear old icons
        foreach (var icon in turnOrderIcons) Destroy(icon);
        turnOrderIcons.Clear();

        if (turnOrderContainer == null || turnOrderIconPrefab == null) return;

        foreach (var character in order)
        {
            var icon = Instantiate(turnOrderIconPrefab, turnOrderContainer);
            var iconComp = icon.GetComponent<TurnOrderIcon>();
            if (iconComp != null) iconComp.Setup(character);
            turnOrderIcons.Add(icon);
        }
    }

    #endregion

    #region Skill Points

    public void UpdateSkillPoints(int current)
    {
        if (skillPointText != null)
            skillPointText.text = $"{current}/{BattleManager.Instance.maxSkillPoints}";

        for (int i = 0; i < skillPips.Count; i++)
        {
            var img = skillPips[i].GetComponent<Image>();
            if (img != null) img.color = i < current ? Color.yellow : new Color(0.3f, 0.3f, 0.3f, 0.7f);
        }
    }

    #endregion

    #region Damage Numbers

    public void ShowDamageNumber(Vector3 worldPos, int damage, bool isCrit, bool isHeal = false)
    {
        if (damageNumberPrefab == null || worldCanvas == null) return;

        var dmgObj = Instantiate(damageNumberPrefab, worldCanvas.transform);
        var dmgComp = dmgObj.GetComponent<DamageNumber>();
        if (dmgComp != null) dmgComp.Show(worldPos, damage, isCrit, isHeal);
    }

    #endregion

    #region Target Selection

    public void StartTargetSelection(List<BattleCharacter> validTargets, System.Action<BattleCharacter> onSelected)
    {
        isSelectingTarget = true;
        onTargetSelected = onSelected;
        foreach (var enemy in validTargets)
        {
            var card = enemyCards.ContainsKey(enemy) ? enemyCards[enemy] : null;
            card?.SetHighlight(true);
        }
    }

    public void OnTargetClicked(BattleCharacter target)
    {
        if (!isSelectingTarget) return;
        isSelectingTarget = false;
        selectedTarget = target;
        ClearTargetHighlights();
        onTargetSelected?.Invoke(target);
        onTargetSelected = null;
    }

    private void ClearTargetHighlights()
    {
        foreach (var card in enemyCards.Values) card?.SetHighlight(false);
    }

    #endregion

    #region Buttons

    private void ShowActionButtons(BattleCharacter character)
    {
        btnBasicAttack.gameObject.SetActive(true);
        btnSkill.gameObject.SetActive(true);
        btnUltimate.gameObject.SetActive(true);

        // Disable skill if not enough SP
        bool canSkill = BattleManager.Instance.currentSkillPoints >= (character.data?.skill?.skillPointCost ?? 1)
                        && character.CanUseSkill();
        btnSkill.interactable = canSkill;

        // Ultimate requires full energy
        bool canUlt = character.currentEnergy >= character.maxEnergy && character.CanUseUltimate();
        btnUltimate.interactable = canUlt;
    }

    private void HideActionButtons()
    {
        btnBasicAttack.gameObject.SetActive(false);
        btnSkill.gameObject.SetActive(false);
        btnUltimate.gameObject.SetActive(false);
    }

    private void RefreshUltimateGlow(BattleCharacter character)
    {
        if (ultimateReadyGlow == null) return;
        bool ready = character.currentEnergy >= character.maxEnergy;
        ultimateReadyGlow.gameObject.SetActive(ready);
    }

    private void OnBasicAttackClicked()
    {
        var skill = BattleManager.Instance.activeCharacter?.data?.basicAttack;
        var enemies = BattleManager.Instance.enemyParty;
        var target = enemies.Find(e => e.isAlive);
        if (skill != null && target != null)
            BattleManager.Instance.PlayerUseSkill(skill, target);
    }

    private void OnSkillClicked()
    {
        var skill = BattleManager.Instance.activeCharacter?.data?.skill;
        var enemies = BattleManager.Instance.enemyParty;
        var target = enemies.Find(e => e.isAlive);
        if (skill != null && target != null)
            BattleManager.Instance.PlayerUseSkill(skill, target);
    }

    private void OnUltimateClicked()
    {
        var skill = BattleManager.Instance.activeCharacter?.data?.ultimate;
        var enemies = BattleManager.Instance.enemyParty;
        var target = enemies.Find(e => e.isAlive);
        if (skill != null && target != null)
            BattleManager.Instance.PlayerUseSkill(skill, target);
    }

    #endregion

    #region Results

    public void ShowVictory() { if (victoryPanel != null) victoryPanel.SetActive(true); }
    public void ShowDefeat() { if (defeatPanel != null) defeatPanel.SetActive(true); }

    private void HideResultPanels()
    {
        if (victoryPanel != null) victoryPanel.SetActive(false);
        if (defeatPanel != null) defeatPanel.SetActive(false);
    }

    #endregion

    #region Element Color Helper

    public Color GetElementColor(ElementType element)
    {
        return element switch
        {
            ElementType.Fire => fireColor,
            ElementType.Ice => iceColor,
            ElementType.Lightning => lightningColor,
            ElementType.Wind => windColor,
            ElementType.Physical => physicalColor,
            _ => Color.white,
        };
    }

    #endregion
}
