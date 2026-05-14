using System.Collections;
using UnityEngine;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance { get; private set; }

    [Header("Referências")]
    public DialogueUI ui;

    public bool IsOpen { get; private set; }

    private DialogueData currentData;
    private int lineIndex;
    private WorldSpaceBubble currentBubble;

    void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
    }

    public void StartDialogue(DialogueData data, WorldSpaceBubble bubble = null)
    {
        currentData = data;
        lineIndex = 0;
        currentBubble = bubble;
        IsOpen = true;

        ui.Show();
        ShowLine();
    }

    void ShowLine()
    {
        if (lineIndex >= currentData.lines.Length) { EndDialogue(); return; }

        DialogueLine line = currentData.lines[lineIndex];
        ui.DisplayLine(line, OnLineFinished);

        currentBubble?.ShowText(line.text);
    }

    // chamado pelo UI quando o typewriter termina
    void OnLineFinished(DialogueLine line)
    {
        if (line.choices != null && line.choices.Length > 0)
            ui.ShowChoices(line.choices, OnChoiceSelected);
        else
            ui.WaitForInput(Advance);
    }

    void Advance()
    {
        lineIndex++;
        ShowLine();
    }

    void OnChoiceSelected(DialogueChoice choice)
    {
        ui.HideChoices();

        if (choice.nextDialogue != null)
        {
            // branching: inicia outro diálogo
            currentData = choice.nextDialogue;
            lineIndex = 0;
            ShowLine();
        }
        else
        {
            EndDialogue();
        }
    }

    public void EndDialogue()
    {
        IsOpen = false;
        ui.Hide();
        currentBubble?.HideText();
    }
}