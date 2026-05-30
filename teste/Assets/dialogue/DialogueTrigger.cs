using UnityEngine;

public class DialogueTrigger : MonoBehaviour
{
    [Header("Dialogue")]
    public DialogueData dialogue;

    [Header("Proximity")]
    public float triggerRadius = 2.5f;
    public KeyCode interactKey = KeyCode.E;

    [Header("World Bubble (optional)")]
    public WorldSpaceBubble bubble;

    private bool playerInRange = false;
    private Transform player;

    void Update()
    {
        if (playerInRange && Input.GetKeyDown(interactKey))
        {
            if (!DialogueManager.Instance.IsOpen)
                DialogueManager.Instance.StartDialogue(dialogue, bubble);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
            playerInRange = false;
    }

    // Visualize radius in editor
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, triggerRadius);
    }
}