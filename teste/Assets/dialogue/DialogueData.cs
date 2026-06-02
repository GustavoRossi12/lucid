using UnityEngine;

[CreateAssetMenu(fileName = "NewDialogue", menuName = "Dialogue/New Dialogue")]
public class DialogueData : ScriptableObject
{
    public DialogueNode[] nodes;
}

[System.Serializable]
public class DialogueNode
{
    public string speakerName;
    public Sprite portrait;
    [TextArea(2, 5)]
    public string text;
}
