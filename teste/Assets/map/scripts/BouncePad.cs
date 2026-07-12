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
        bool isPlayer = other.CompareTag("Player");
        var block = other.GetComponentInParent<PushBlock>();

        if (!isPlayer && block == null) return;

        if (bounceVFXPrefab != null)
            Instantiate(bounceVFXPrefab, transform.position, Quaternion.Euler(bounceVFXAngle));

        if (isPlayer)
        {
            if (other.TryGetComponent<ParkourController>(out var pc))
                pc.AddInertia(transform.up * launchForce);
        }
        else if (block != null)
        {
            var rb = block.GetComponent<Rigidbody>();
            if (rb != null)
                rb.linearVelocity = new Vector3(rb.linearVelocity.x, launchForce, rb.linearVelocity.z);
        }
    }
}
