using UnityEngine;

public class SpinnerLauncher : MonoBehaviour
{
    [Header("Rotação")]
    public float rotateSpeed = 180f;
    public Vector3 rotateAxis = Vector3.up;

    [Header("Empurrão")]
    public float knockbackForce = 15f;
    public float upwardForce = 5f;

    void Update()
    {
        transform.Rotate(rotateAxis * rotateSpeed * Time.deltaTime);
    }

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        Vector3 dir = (other.transform.position - transform.position).normalized;
        dir.y = 0f;
        dir.Normalize();

        Vector3 force = dir * knockbackForce + Vector3.up * upwardForce;

        if (other.TryGetComponent<CharacterController>(out var cc))
        {
            StartCoroutine(KnockbackRoutine(cc, force));
        }
    }

    private System.Collections.IEnumerator KnockbackRoutine(CharacterController cc, Vector3 force)
    {
        float duration = 0.3f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            cc.Move(force * Time.deltaTime);
            elapsed += Time.deltaTime;
            yield return null;
        }
    }
}
