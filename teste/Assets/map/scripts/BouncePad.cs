using UnityEngine;

public class BouncePad : MonoBehaviour
{
    [Header("Impulso")]
    public float launchForce = 10f;

    [Header("VFX")]
    public GameObject bounceVFXPrefab;
    public Vector3 bounceVFXAngle = Vector3.zero;

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        if (bounceVFXPrefab != null)
            Instantiate(bounceVFXPrefab, transform.position, Quaternion.Euler(bounceVFXAngle));

        if (other.TryGetComponent<ParkourController>(out var pc))
        {
            pc.ApplyKnockback(transform.up * launchForce);
        }
    }
}
