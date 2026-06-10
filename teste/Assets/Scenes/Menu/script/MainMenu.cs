using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [Header("Botões")]
    public GameObject continueButton;

    void Start()
    {
        if (continueButton != null)
            continueButton.SetActive(SaveSystem.HasSave);
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

    public void QuitGame()
    {
        Application.Quit();
    }
}
