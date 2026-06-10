using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DialogueUI : MonoBehaviour
{
    [Header("Main Panel")]
    public GameObject panel;
    public CanvasGroup canvasGroup;
    public TMP_Text speakerText;
    public TMP_Text dialogueText;
    public Image portraitImage;
    public GameObject portraitContainer;

    [Header("Typewriter")]
    public float typeSpeed = 0.03f;

    [Header("Fade")]
    public float fadeSpeed = 4f;

    [Header("Continue Hint")]
    public GameObject continueHint;

    private Coroutine typeCoroutine;
    private Coroutine fadeCoroutine;
    private Action waitCallback;

    void Awake()
    {
        if (canvasGroup == null) canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup != null) canvasGroup.alpha = 0f;
        panel.SetActive(false);
    }

    public void Show()
    {
        gameObject.SetActive(true);
        panel.SetActive(true);
        if (fadeCoroutine != null) StopCoroutine(fadeCoroutine);
        fadeCoroutine = StartCoroutine(FadeIn());
    }

    public void Hide()
    {
        if (fadeCoroutine != null) StopCoroutine(fadeCoroutine);
        fadeCoroutine = StartCoroutine(FadeOut());
    }

    IEnumerator FadeIn()
    {
        if (canvasGroup == null) yield break;
        canvasGroup.alpha = 0f;
        while (canvasGroup.alpha < 1f)
        {
            canvasGroup.alpha += fadeSpeed * Time.deltaTime;
            yield return null;
        }
        canvasGroup.alpha = 1f;
    }

    IEnumerator FadeOut()
    {
        if (canvasGroup == null) { panel.SetActive(false); gameObject.SetActive(false); yield break; }
        while (canvasGroup.alpha > 0f)
        {
            canvasGroup.alpha -= fadeSpeed * Time.deltaTime;
            yield return null;
        }
        canvasGroup.alpha = 0f;
        panel.SetActive(false);
        gameObject.SetActive(false);
    }

    public void DisplayNode(DialogueNode node, Action<DialogueNode> onDone)
    {
        continueHint.SetActive(false);
        speakerText.text = node.speakerName;

        if (node.portrait != null && portraitImage != null)
        {
            if (portraitContainer != null) portraitContainer.SetActive(true);
            portraitImage.sprite = node.portrait;
        }
        else
        {
            if (portraitContainer != null) portraitContainer.SetActive(false);
        }

        if (typeCoroutine != null) StopCoroutine(typeCoroutine);
        typeCoroutine = StartCoroutine(TypewriterRoutine(node, onDone));
    }

    IEnumerator TypewriterRoutine(DialogueNode node, Action<DialogueNode> onDone)
    {
        dialogueText.text = "";

        foreach (char c in node.text)
        {
            dialogueText.text += c;
            yield return new WaitForSeconds(typeSpeed);
        }
        onDone?.Invoke(node);
    }

    public void WaitForInput(Action callback)
    {
        continueHint.SetActive(true);
        waitCallback = callback;
        StartCoroutine(WaitInputRoutine());
    }

    IEnumerator WaitInputRoutine()
    {
        yield return new WaitUntil(() => !Input.GetKeyDown(KeyCode.E));
        yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.E));
        continueHint.SetActive(false);
        waitCallback?.Invoke();
    }
}
