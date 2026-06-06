using UnityEngine;

public class CollectiblePoint : MonoBehaviour
{
    [Header("Pontos")]
    public int pointAmount = 1;

    [Header("VFX")]
    public GameObject idleVFXPrefab;
    public Vector3 idleVFXAngle = Vector3.zero;
    public GameObject collectVFXPrefab;
    public Vector3 collectVFXAngle = Vector3.zero;

    [Header("Flutuação")]
    public float floatAmplitude = 0.3f;
    public float floatFrequency = 2f;

    private Camera cachedCamera;
    private Vector3 startPos;

    void Start()
    {
        cachedCamera = Camera.main;
        startPos = transform.position;

        if (idleVFXPrefab != null)
        {
            GameObject vfx = Instantiate(idleVFXPrefab, transform.position, Quaternion.Euler(idleVFXAngle));
            vfx.transform.SetParent(transform);
        }
    }

    void Update()
    {
        if (cachedCamera == null) return;

        Vector3 lookDir = cachedCamera.transform.forward;
        lookDir.y = 0f;
        transform.rotation = Quaternion.LookRotation(lookDir);

        Vector3 pos = startPos;
        pos.y += Mathf.Sin(Time.time * floatFrequency) * floatAmplitude;
        transform.position = pos;
    }

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        if (other.TryGetComponent<PlayerStats>(out var stats))
        {
            stats.AddPoints(pointAmount);
        }

        if (collectVFXPrefab != null)
            Instantiate(collectVFXPrefab, transform.position, Quaternion.Euler(collectVFXAngle));

        Destroy(gameObject);
    }
}
