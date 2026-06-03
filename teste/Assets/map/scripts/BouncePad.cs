using UnityEngine;

public class BouncePad : MonoBehaviour
{
    [Header("Impulso")]
    public float launchForce = 10f;

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        Vector3 direction = transform.up;

        if (other.TryGetComponent<CharacterController>(out var cc))
        {
            StartCoroutine(LaunchRoutine(cc, direction * launchForce));
        }
    }

    private System.Collections.IEnumerator LaunchRoutine(CharacterController cc, Vector3 force)
    {
        float duration = 0.2f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            cc.Move(force * Time.deltaTime);
            elapsed += Time.deltaTime;
            yield return null;
        }
    }
}
