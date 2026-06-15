using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    [Header("UI")]
    public GameObject pausePanel;

    private bool isPaused;
    private ParkourController playerController;
    private CameraOrbit cameraOrbit;

    private SaveManager saveManager;

    void Start()
    {
        if (pausePanel != null) pausePanel.SetActive(false);
        saveManager = FindAnyObjectByType<SaveManager>();
    }

    void Update()
    {
        if (!Input.GetKeyDown(KeyCode.Escape)) return;

        if (DialogueManager.Instance != null && DialogueManager.Instance.IsOpen) return;

        if (FindAnyObjectByType<UpgradeUI>() is UpgradeUI ui && ui.IsOpen) return;

        if (isPaused) Resume();
        else Pause();
    }

    void Pause()
    {
        isPaused = true;
        Time.timeScale = 0f;

        if (playerController == null)
            playerController = FindAnyObjectByType<ParkourController>();
        if (cameraOrbit == null)
            cameraOrbit = FindAnyObjectByType<CameraOrbit>();

        if (playerController != null)
        {
            playerController.canLook = false;
            playerController.canMove = false;
        }
        if (cameraOrbit != null) cameraOrbit.enabled = false;

        if (pausePanel != null) pausePanel.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void Resume()
    {
        isPaused = false;
        Time.timeScale = 1f;

        if (playerController != null)
        {
            playerController.canLook = true;
            playerController.canMove = true;
        }
        if (cameraOrbit != null) cameraOrbit.enabled = true;

        if (pausePanel != null) pausePanel.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void SaveGame()
    {
        if (saveManager != null) saveManager.SaveGame();
    }

    public void LoadMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Menu");
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
