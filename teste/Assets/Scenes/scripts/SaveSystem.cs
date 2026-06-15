using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

[System.Serializable]
public class SaveData
{
    public string sceneName;
    public float playerX, playerY, playerZ;
    public int points;
    public int speedLevel, dashLevel, jumpLevel;
    public int totalVisits;
}

public static class SaveSystem
{
    private static string Path => Application.persistentDataPath + "/save.dat";

    public static void Save(SaveData data)
    {
        using (FileStream stream = new FileStream(Path, FileMode.Create))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            formatter.Serialize(stream, data);
        }
    }

    public static SaveData Load()
    {
        if (!File.Exists(Path)) return null;

        using (FileStream stream = new FileStream(Path, FileMode.Open))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            return formatter.Deserialize(stream) as SaveData;
        }
    }

    public static bool HasSave => File.Exists(Path);

    public static void Delete()
    {
        if (File.Exists(Path)) File.Delete(Path);
    }
}
