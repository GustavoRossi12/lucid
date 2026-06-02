using UnityEngine;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance { get; private set; }

    [Header("References")]
    public DialogueUI ui;

    public bool IsOpen { get; private set; }

    private DialogueData currentData;
    private int currentNodeIndex;

    void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
    }

    public void StartDialogue(DialogueData data)
    {
        currentData = data;
        currentNodeIndex = data.nodes.Length > 0 ? 0 : -1;
        IsOpen = true;
        ui.Show();
        ShowNode();
    }

    void ShowNode()
    {
        if (currentNodeIndex < 0 || currentNodeIndex >= currentData.nodes.Length)
        {
            EndDialogue();
            return;
        }

        DialogueNode node = currentData.nodes[currentNodeIndex];
        ui.DisplayNode(node, OnNodeFinished);
    }

    void OnNodeFinished(DialogueNode node)
    {
        ui.WaitForInput(Advance);
    }

    void Advance()
    {
        currentNodeIndex++;
        ShowNode();
    }

    public void EndDialogue()
    {
        IsOpen = false;
        ui.Hide();
    }
}
