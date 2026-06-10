using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [Header("Botões")]
    public GameObject continueButton;

    [Header("Menus")]
    public GameObject mainPanel;
    public GameObject settingsPanel;

    void Start()
    {
        if (continueButton != null)
            continueButton.SetActive(SaveSystem.HasSave);

        if (settingsPanel != null) settingsPanel.SetActive(false);
        if (mainPanel != null) mainPanel.SetActive(true);
    }

    public void NewGame()
    {
        SaveSystem.Delete();
        SceneManager.LoadScene("SampleScene");
    }

    public void Continue()
    {
        SceneManager.LoadScene("SampleScene");
    }

    public void OpenSettings()
    {
        if (mainPanel != null) mainPanel.SetActive(false);
        if (settingsPanel != null) settingsPanel.SetActive(true);
    }

    public void CloseSettings()
    {
        if (settingsPanel != null) settingsPanel.SetActive(false);
        if (mainPanel != null) mainPanel.SetActive(true);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
