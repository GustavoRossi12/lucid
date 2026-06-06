using UnityEngine;

public class SpinnerLauncher : MonoBehaviour
{
    [Header("Rotação")]
    public float rotateSpeed = 180f;
    public Vector3 rotateAxis = Vector3.up;

    [Header("Empurrão")]
    public float knockbackForce = 15f;
    public float upwardForce = 5f;

    [Header("VFX")]
    public GameObject hitVFXPrefab;
    public Vector3 hitVFXAngle = Vector3.zero;

    void Update()
    {
        transform.Rotate(rotateAxis * rotateSpeed * Time.deltaTime);
    }

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        if (hitVFXPrefab != null)
            Instantiate(hitVFXPrefab, other.transform.position, Quaternion.Euler(hitVFXAngle));

        Vector3 dir = (other.transform.position - transform.position).normalized;
        dir.y = 0f;
        dir.Normalize();

        Vector3 force = dir * knockbackForce + Vector3.up * upwardForce;

        if (other.TryGetComponent<ParkourController>(out var pc))
        {
            pc.ApplyKnockback(force);
        }
    }
}
