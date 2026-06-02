using UnityEngine;

[RequireComponent(typeof(Collider))]
public class DialogueTrigger : MonoBehaviour
{
    [Header("Dialogue")]
    public DialogueData dialogue;

    [Header("Proximity")]
    public float triggerRadius = 2.5f;
    public KeyCode interactKey = KeyCode.E;

    private bool playerInRange;
    private Collider triggerCollider;

    void Awake()
    {
        triggerCollider = GetComponent<Collider>();
        triggerCollider.isTrigger = true;
        if (triggerCollider is SphereCollider sphere)
            sphere.radius = triggerRadius;
    }

    void Update()
    {
        if (playerInRange && Input.GetKeyDown(interactKey))
        {
            if (!DialogueManager.Instance.IsOpen)
                DialogueManager.Instance.StartDialogue(dialogue);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            playerInRange = true;
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
            playerInRange = false;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        float radius = triggerRadius;
        if (GetComponent<SphereCollider>() is SphereCollider sc)
            radius = sc.radius;
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}
