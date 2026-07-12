using UnityEngine;
using UnityEngine.SceneManagement;

public class SaveManager : MonoBehaviour
{
    public static SaveManager Instance { get; private set; }

    void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
    }

    void Start()
    {
        if (SaveSystem.HasSave)
            ApplyData(SaveSystem.Load());
    }

    public void SaveGame()
    {
        SaveData data = new SaveData();
        data.sceneName = SceneManager.GetActiveScene().name;

        PlayerStats stats = FindAnyObjectByType<PlayerStats>();
        if (stats != null)
        {
            data.points = stats.points;
            data.collectedItems = new string[stats.collected.Count];
            for (int i = 0; i < stats.collected.Count; i++)
                data.collectedItems[i] = stats.collected[i] != null ? stats.collected[i].name : "";
        }

        Transform player = GameObject.FindGameObjectWithTag("Player")?.transform;
        if (player != null)
        {
            data.playerX = player.position.x;
            data.playerY = player.position.y;
            data.playerZ = player.position.z;
        }

        SaveSystem.Save(data);
    }

    private void ApplyData(SaveData data)
    {
        PlayerStats stats = FindAnyObjectByType<PlayerStats>();
        if (stats != null)
        {
            stats.points = data.points;
            stats.collected.Clear();
            if (data.collectedItems != null)
            {
                foreach (string itemName in data.collectedItems)
                {
                    foreach (var pool in stats.collectiblePool)
                    {
                        if (pool != null && pool.name == itemName)
                        {
                            stats.collected.Add(pool);
                            break;
                        }
                    }
                }
            }
        }

        Transform player = GameObject.FindGameObjectWithTag("Player")?.transform;
        if (player != null)
        {
            player.position = new Vector3(data.playerX, data.playerY, data.playerZ);
        }
    }
}
