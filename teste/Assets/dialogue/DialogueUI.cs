using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DialogueUI : MonoBehaviour
{
    [Header("Painel principal")]
    public GameObject panel;
    public TMP_Text speakerText;
    public TMP_Text dialogueText;
    public Image portraitImage;
    public GameObject portraitContainer;

    [Header("Typewriter")]
    public float typeSpeed = 0.03f; // segundos por letra

    [Header("Dica de avançar")]
    public GameObject continueHint; // ex: texto "[E] continuar"

    [Header("Escolhas")]
    public GameObject choicesContainer;
    public Button choiceButtonPrefab;

    private Coroutine typeCoroutine;
    private bool typewriterDone = false;
    private Action waitCallback;

    // ── Mostrar/esconder ──────────────────────────────────────────────────────

    public void Show() => panel.SetActive(true);
    public void Hide()
    {
        panel.SetActive(false);
        HideChoices();
    }

    // ── Exibir linha ──────────────────────────────────────────────────────────

    public void DisplayLine(DialogueLine line, Action<DialogueLine> onDone)
    {
        continueHint.SetActive(false);
        HideChoices();

        speakerText.text = line.speakerName;

        // retrato
        if (line.portrait != null)
        {
            portraitContainer.SetActive(true);
            portraitImage.sprite = line.portrait;
        }
        else
        {
            portraitContainer.SetActive(false);
        }

        if (typeCoroutine != null) StopCoroutine(typeCoroutine);
        typeCoroutine = StartCoroutine(TypewriterRoutine(line, onDone));
    }

    IEnumerator TypewriterRoutine(DialogueLine line, Action<DialogueLine> onDone)
    {
        typewriterDone = false;
        dialogueText.text = "";

        foreach (char c in line.text)
        {
            dialogueText.text += c;
            yield return new WaitForSeconds(typeSpeed);
        }

        typewriterDone = true;
        onDone?.Invoke(line);
    }

    // ── Avançar com input ─────────────────────────────────────────────────────

    public void WaitForInput(Action callback)
    {
        continueHint.SetActive(true);
        waitCallback = callback;
        StartCoroutine(WaitInputRoutine());
    }

    IEnumerator WaitInputRoutine()
    {
        // aguarda soltar a tecla para não capturar o pressionamento anterior
        yield return new WaitUntil(() => !Input.GetKeyDown(KeyCode.E));
        yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.E));
        continueHint.SetActive(false);
        waitCallback?.Invoke();
    }

    // ── Escolhas ──────────────────────────────────────────────────────────────

    public void ShowChoices(DialogueChoice[] choices, Action<DialogueChoice> onSelect)
    {
        HideChoices();
        choicesContainer.SetActive(true);

        foreach (var choice in choices)
        {
            Button btn = Instantiate(choiceButtonPrefab, choicesContainer.transform);
            btn.GetComponentInChildren<TMP_Text>().text = choice.label;

            // captura local para o closure
            var localChoice = choice;
            btn.onClick.AddListener(() => onSelect(localChoice));
        }
    }

    public void HideChoices()
    {
        foreach (Transform child in choicesContainer.transform)
            Destroy(child.gameObject);
        choicesContainer.SetActive(false);
    }
}