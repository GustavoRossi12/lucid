using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneRandomizer : MonoBehaviour
{
    [Header("Salas Aleatórias")]
    [Tooltip("Nomes das cenas na pasta 'salas aleatorias'")]
    public string[] randomScenes;

    [Header("História")]
    [Tooltip("Após X visitas, carrega uma cena específica da pasta 'historia'")]
    public StoryMilestone[] storyMilestones;

    [Header("UI do Contador")]
    public TMP_Text counterText;

    [Header("Estado")]
    public int totalVisits;

    void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        UpdateCounterUI();
    }

    public void LoadRandomScene()
    {
        totalVisits++;
        UpdateCounterUI();

        for (int i = 0; i < storyMilestones.Length; i++)
        {
            if (storyMilestones[i].visitCount == totalVisits)
            {
                SceneManager.LoadScene(storyMilestones[i].sceneName);
                return;
            }
        }

        if (randomScenes == null || randomScenes.Length == 0)
        {
            Debug.LogWarning("Nenhuma cena aleatória configurada.");
            return;
        }

        string randomScene = randomScenes[Random.Range(0, randomScenes.Length)];
        SceneManager.LoadScene(randomScene);
    }

    private void UpdateCounterUI()
    {
        if (counterText != null)
            counterText.text = $"Salas visitadas: {totalVisits}";
    }
}

[System.Serializable]
public class StoryMilestone
{
    public int visitCount;
    public string sceneName;
}
