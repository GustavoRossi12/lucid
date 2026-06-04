using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DialogueUI : MonoBehaviour
{
    [Header("Main Panel")]
    public GameObject panel;
    public TMP_Text speakerText;
    public TMP_Text dialogueText;
    public Image portraitImage;
    public GameObject portraitContainer;

    [Header("Typewriter")]
    public float typeSpeed = 0.03f;

    [Header("Continue Hint")]
    public GameObject continueHint;

    private Coroutine typeCoroutine;
    private Action waitCallback;

    public void Show()
    {
        gameObject.SetActive(true);
        panel.SetActive(true);
    }

    public void Hide()
    {
        panel.SetActive(false);
        gameObject.SetActive(false);
    }

    public void DisplayNode(DialogueNode node, Action<DialogueNode> onDone)
    {
        continueHint.SetActive(false);

        speakerText.text = node.speakerName;

        if (node.portrait != null)
        {
            portraitContainer.SetActive(true);
            portraitImage.sprite = node.portrait;
        }
        else
        {
            portraitContainer.SetActive(false);
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
