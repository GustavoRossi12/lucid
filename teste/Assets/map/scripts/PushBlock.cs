using UnityEngine;

[RequireComponent(typeof(Rigidbody), typeof(BoxCollider))]
public class PushBlock : MonoBehaviour
{
    public float pushForce = 10f;
    public float friction = 3f;
    private Rigidbody rb;
    private BoxCollider box;
    private RigidbodyConstraints defaultConstraints;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        box = GetComponent<BoxCollider>();
        defaultConstraints = RigidbodyConstraints.FreezeRotation;
        rb.constraints = defaultConstraints;
    }

    public void Push(Vector3 direction)
    {
        Vector3 target = direction * pushForce;
        rb.linearVelocity = new Vector3(target.x, rb.linearVelocity.y, target.z);
    }

    void FixedUpdate()
    {
        Vector3 vel = rb.linearVelocity;
        vel.x = Mathf.Lerp(vel.x, 0f, friction * Time.fixedDeltaTime);
        vel.z = Mathf.Lerp(vel.z, 0f, friction * Time.fixedDeltaTime);
        rb.linearVelocity = vel;

        Vector3 topCenter = transform.position + box.center + Vector3.up * box.size.y * 0.5f;
        Vector3 halfExtents = new Vector3(box.size.x * 0.4f, 0.05f, box.size.z * 0.4f);
        Collider[] onTop = Physics.OverlapBox(topCenter, halfExtents, transform.rotation);
        bool playerOnTop = false;
        foreach (var c in onTop)
        {
            if (c.CompareTag("Player")) { playerOnTop = true; break; }
        }

        rb.constraints = playerOnTop
            ? defaultConstraints | RigidbodyConstraints.FreezePositionY
            : defaultConstraints;
    }
}
