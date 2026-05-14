using UnityEngine;

[CreateAssetMenu(fileName = "NewLine", menuName = "Dialogue/Dialogue Line")]
public class DialogueLine : ScriptableObject
{
    [Header("Personagem")]
    public string speakerName;
    public Sprite portrait;

    [Header("Texto")]
    [TextArea(2, 5)]
    public string text;

    [Header("Escolhas (deixe vazio para avançar direto)")]
    public DialogueChoice[] choices;
}

[System.Serializable]
public class DialogueChoice
{
    public string label;
    public DialogueData nextDialogue; // null = encerra
}