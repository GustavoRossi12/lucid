using UnityEngine;

[CreateAssetMenu(fileName = "NewLine", menuName = "Dialogue/Dialogue Line")]
public class DialogueLine : ScriptableObject
{
    [Header("Character")]
    public string speakerName;
    public Sprite portrait;

    [Header("Text")]
    [TextArea(2, 5)]
    public string text;

    [Header("Choices (leave empty to advance)")]
    public DialogueChoice[] choices;
}

[System.Serializable]
public class DialogueChoice
{
    public string label;
    public DialogueData nextDialogue;
}