using UnityEngine;

public class PlayerVFX : MonoBehaviour
{
    [Header("Jump VFX")]
    public GameObject jumpVFXPrefab;
    public Vector3 jumpVFXAngle = new Vector3(-90f, 0f, 0f);

    [Header("Dash VFX")]
    public GameObject dashVFXPrefab;
    public Vector3 dashVFXAngle = Vector3.zero;

    [Header("Run VFX")]
    public GameObject runVFXPrefab;
    public Vector3 runVFXAngle = new Vector3(-90f, 0f, 0f);
    public float runVFXInterval = 0.4f;

    private ParkourController controller;
    private float runTimer;

    void Start()
    {
        controller = GetComponent<ParkourController>();
        if (controller != null)
        {
            controller.OnJumped += SpawnJumpVFX;
            controller.OnDashStarted += SpawnDashVFX;
        }
    }

    void OnDestroy()
    {
        if (controller != null)
        {
            controller.OnJumped -= SpawnJumpVFX;
            controller.OnDashStarted -= SpawnDashVFX;
        }
    }

    void Update()
    {
        HandleRunVFX();
    }

    void HandleRunVFX()
    {
        if (runVFXPrefab == null || controller == null) return;

        if (controller.IsGrounded && controller.IsMoving && !controller.EstaDashando)
        {
            runTimer += Time.deltaTime;
            if (runTimer >= runVFXInterval)
            {
                SpawnVFX(runVFXPrefab, runVFXAngle);
                runTimer = 0f;
            }
        }
        else
        {
            runTimer = runVFXInterval;
        }
    }

    void SpawnJumpVFX()
    {
        SpawnVFX(jumpVFXPrefab, jumpVFXAngle);
    }

    void SpawnDashVFX()
    {
        SpawnVFX(dashVFXPrefab, dashVFXAngle);
    }

    void SpawnVFX(GameObject prefab, Vector3 angle)
    {
        if (prefab == null) return;
        Instantiate(prefab, transform.position, Quaternion.Euler(angle));
    }
}
