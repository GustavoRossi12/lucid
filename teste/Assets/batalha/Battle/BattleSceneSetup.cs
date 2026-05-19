using System.Collections;
using UnityEngine;

/// <summary>
/// Attach this to a BattleScene GameObject.
/// Assign party and enemy CharacterData assets, then hit Play.
/// This bootstrapper instantiates characters and starts the battle.
/// </summary>
public class BattleSceneSetup : MonoBehaviour
{
    [Header("Player Party Setup")]
    public CharacterData[] partyData;
    public Transform[] partySpawnPoints;
    public GameObject battleCharacterPrefab;

    [Header("Enemy Setup")]
    public CharacterData[] enemyData;
    public Transform[] enemySpawnPoints;
    public GameObject enemyCharacterPrefab;

    [Header("Level")]
    public int partyLevel = 20;
    public int enemyLevel = 18;

    [Header("Auto Start")]
    public bool autoStartBattle = true;
    public float startDelay = 0.5f;

    private void Start()
    {
        if (autoStartBattle)
            StartCoroutine(SetupAndStartBattle());
    }

    public IEnumerator SetupAndStartBattle()
    {
        yield return new WaitForSeconds(startDelay);

        // Instantiate party
        for (int i = 0; i < partyData.Length; i++)
        {
            if (partyData[i] == null) continue;
            Vector3 pos = i < partySpawnPoints.Length ? partySpawnPoints[i].position : Vector3.left * (2 - i);

            var prefab = battleCharacterPrefab != null ? battleCharacterPrefab : new GameObject();
            var go = Instantiate(prefab, pos, Quaternion.identity);
            go.name = partyData[i].characterName;

            var character = go.GetComponent<BattleCharacter>() ?? go.AddComponent<BattleCharacter>();
            character.data = partyData[i];
            character.level = partyLevel;
            character.isEnemy = false;
            character.InitializeFromData();

            BattleManager.Instance.playerParty.Add(character);
        }

        // Instantiate enemies
        for (int i = 0; i < enemyData.Length; i++)
        {
            if (enemyData[i] == null) continue;
            Vector3 pos = i < enemySpawnPoints.Length ? enemySpawnPoints[i].position : Vector3.right * (2 + i);

            var prefab = enemyCharacterPrefab != null ? enemyCharacterPrefab : new GameObject();
            var go = Instantiate(prefab, pos, Quaternion.identity);
            go.name = enemyData[i].characterName + " (Enemy)";

            var character = go.GetComponent<BattleCharacter>() ?? go.AddComponent<BattleCharacter>();
            character.data = enemyData[i];
            character.level = enemyLevel;
            character.isEnemy = true;
            character.InitializeFromData();

            var ai = go.AddComponent<EnemyAI>();
            ai.behavior = AIBehavior.Balanced;

            BattleManager.Instance.enemyParty.Add(character);
        }

        // Wire UI
        if (BattleUIManager.Instance != null)
        {
            BattleUIManager.Instance.InitializeBattle(
                BattleManager.Instance.playerParty,
                BattleManager.Instance.enemyParty
            );
        }

        // Wire BattleManager events to UI
        WireEvents();

        // Start!
        BattleManager.Instance.StartBattle();
    }

    private void WireEvents()
    {
        var bm = BattleManager.Instance;
        var ui = BattleUIManager.Instance;
        if (bm == null || ui == null) return;

        bm.onTurnStart.AddListener(character =>
        {
            if (character.isEnemy) ui.OnEnemyTurnStart(character);
            else ui.OnPlayerTurnStart(character);
        });

        bm.onSkillPointsChanged.AddListener(sp => ui.UpdateSkillPoints(sp));
        bm.onTurnOrderUpdated.AddListener(order => ui.UpdateTurnOrder(order));

        bm.onBattleResult.AddListener(result =>
        {
            if (result == BattleResult.Victory) ui.ShowVictory();
            else if (result == BattleResult.Defeat) ui.ShowDefeat();
        });
    }
}
