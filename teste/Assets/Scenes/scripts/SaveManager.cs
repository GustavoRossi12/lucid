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
            data.speedLevel = stats.speedLevel;
            data.dashLevel = stats.dashLevel;
            data.jumpLevel = stats.jumpLevel;
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
            stats.speedLevel = data.speedLevel;
            stats.dashLevel = data.dashLevel;
            stats.jumpLevel = data.jumpLevel;
        }

        ParkourController controller = FindAnyObjectByType<ParkourController>();
        if (controller != null)
        {
            controller.maxSpeed = 15f + data.speedLevel * 1f;
            controller.dashForce = 30f + data.dashLevel * 3f;
            controller.jumpHeight = 2f + data.jumpLevel * 0.5f;
        }

        Transform player = GameObject.FindGameObjectWithTag("Player")?.transform;
        if (player != null)
        {
            player.position = new Vector3(data.playerX, data.playerY, data.playerZ);
        }
    }
}
