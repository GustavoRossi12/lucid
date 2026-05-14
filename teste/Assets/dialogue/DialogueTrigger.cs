using UnityEngine;

public class DialogueTrigger : MonoBehaviour
{
    [Header("Diálogo")]
    public DialogueData dialogue;

    [Header("Proximidade")]
    public float triggerRadius = 2.5f;
    public KeyCode interactKey = KeyCode.E;

    [Header("Balão 3D (opcional)")]
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
            // mostra dica de interação (opcional: UI hint)
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
            playerInRange = false;
    }

    // Visualiza o raio no editor
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, triggerRadius);
    }
}