using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [Header("Botões")]
    public GameObject continueButton;

    [Header("Menus")]
    public RectTransform mainPanel;
    public RectTransform settingsPanel;
    public CanvasGroup canvasGroup;
    public float slideDistance = 600f;
    public float slideSpeed = 8f;

    private Vector2 mainTarget;
    private Vector2 settingsTarget;

    void Start()
    {
        if (continueButton != null)
            continueButton.SetActive(SaveSystem.HasSave);

        if (canvasGroup == null) canvasGroup = GetComponent<CanvasGroup>();

        if (mainPanel != null)
        {
            mainTarget = mainPanel.anchoredPosition;
            mainPanel.anchoredPosition = mainTarget - new Vector2(slideDistance, 0f);
            StartCoroutine(SlideIn(mainPanel, mainTarget));
        }

        if (settingsPanel != null)
        {
            settingsTarget = settingsPanel.anchoredPosition;
            settingsPanel.anchoredPosition = settingsTarget - new Vector2(slideDistance, 0f);
            settingsPanel.gameObject.SetActive(false);
        }
    }

    IEnumerator SlideIn(RectTransform rect, Vector2 target)
    {
        if (rect == null) yield break;
        if (canvasGroup != null) canvasGroup.alpha = 1f;

        while (Vector2.Distance(rect.anchoredPosition, target) > 1f)
        {
            rect.anchoredPosition = Vector2.Lerp(rect.anchoredPosition, target, slideSpeed * Time.deltaTime);
            yield return null;
        }
        rect.anchoredPosition = target;
    }

    IEnumerator SlideOut(RectTransform rect, Vector2 target, System.Action onDone)
    {
        if (rect == null) { onDone?.Invoke(); yield break; }

        Vector2 hidden = target - new Vector2(slideDistance, 0f);
        while (Vector2.Distance(rect.anchoredPosition, hidden) > 1f)
        {
            rect.anchoredPosition = Vector2.Lerp(rect.anchoredPosition, hidden, slideSpeed * Time.deltaTime);
            yield return null;
        }
        rect.anchoredPosition = hidden;
        onDone?.Invoke();
    }

    void ClickSound()
    {
        if (SoundManager.Instance != null) SoundManager.Instance.MenuClick();
    }

    public void NewGame()
    {
        ClickSound();
        SaveSystem.Delete();
        SceneManager.LoadScene("SampleScene");
    }

    public void Continue()
    {
        ClickSound();
        SceneManager.LoadScene("SampleScene");
    }

    public void OpenSettings()
    {
        ClickSound();
        if (mainPanel != null)
            StartCoroutine(SlideOut(mainPanel, mainTarget, () => mainPanel.gameObject.SetActive(false)));

        if (settingsPanel != null)
        {
            settingsPanel.gameObject.SetActive(true);
            settingsPanel.anchoredPosition = settingsTarget - new Vector2(slideDistance, 0f);
            StartCoroutine(SlideIn(settingsPanel, settingsTarget));
        }
    }

    public void CloseSettings()
    {
        ClickSound();
        if (settingsPanel != null)
            StartCoroutine(SlideOut(settingsPanel, settingsTarget, () => settingsPanel.gameObject.SetActive(false)));

        if (mainPanel != null)
        {
            mainPanel.gameObject.SetActive(true);
            mainPanel.anchoredPosition = mainTarget - new Vector2(slideDistance, 0f);
            StartCoroutine(SlideIn(mainPanel, mainTarget));
        }
    }

    public void QuitGame()
    {
        ClickSound();
        Application.Quit();
    }
}
