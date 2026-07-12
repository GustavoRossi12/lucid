using UnityEngine;
public class CameraOrbit : MonoBehaviour
{
    [Header("Target")]
    public Transform target;
    [Header("Rotation")]
    public float mouseSensitivity = 2f;
    public float horizontalMultiplier = 2f;
    public float fixedPitch = 15f;
    public bool canLook = true;
    float yaw;
    [Header("Zoom")]
    public float defaultFOV = 60f;
    public float minFOV = 20f;
    public float maxFOV = 90f;
    public float fovZoomSpeed = 10f;
    public float fovSmoothing = 8f;
    float targetFOV;
    [Header("Distance")]
    public float distance = 8f;
    public float positionSmoothing = 10f;
    [Header("Height")]
    public float heightOffset = 1.5f;
    [Header("Ceiling Inversion")]
    public bool invertOrbit;
    [Header("Collision")]
    public LayerMask collisionMask;
    public float collisionRadius = 0.3f;
    private Camera cam;
    void Start()
    {
        cam = GetComponentInChildren<Camera>();
        Cursor.lockState = CursorLockMode.Locked;
        mouseSensitivity = Mathf.Max(0.1f, PlayerPrefs.GetFloat("Sensitivity", mouseSensitivity));
        yaw = transform.eulerAngles.y;
        targetFOV = defaultFOV;
        if (cam != null)
            cam.fieldOfView = defaultFOV;
    }
    void LateUpdate()
    {
        if (target == null || cam == null) return;
        HandleMouse();
        HandleZoom();
        UpdateCamera();
    }
    void HandleMouse()
    {
        if (!canLook) return;
        yaw += Input.GetAxis("Mouse X") * mouseSensitivity * horizontalMultiplier;
        yaw = Mathf.Repeat(yaw, 360f);
    }
    void HandleZoom()
    {
        if (!canLook) return;
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        targetFOV -= scroll * fovZoomSpeed;
        targetFOV = Mathf.Clamp(targetFOV, minFOV, maxFOV);
        cam.fieldOfView = Mathf.Lerp(
            cam.fieldOfView,
            targetFOV,
            Time.deltaTime * fovSmoothing
        );
    }
    void UpdateCamera()
    {
        float pitch = invertOrbit ? -fixedPitch : fixedPitch;
        float hOffset = invertOrbit ? -heightOffset : heightOffset;

        Vector3 targetPosition = target.position + Vector3.up * hOffset;
        Quaternion rotation = Quaternion.Euler(pitch, yaw, 0f);
        Vector3 offset = rotation * new Vector3(0f, 0f, -distance);
        Vector3 desiredPosition = targetPosition + offset;
        Vector3 direction = (desiredPosition - targetPosition).normalized;
        float dist = Vector3.Distance(targetPosition, desiredPosition);
        if (Physics.SphereCast(targetPosition, collisionRadius, direction,
                               out RaycastHit hit, dist, collisionMask))
        {
            desiredPosition = hit.point + hit.normal * collisionRadius;
        }
        transform.position = Vector3.Lerp(
            transform.position,
            desiredPosition,
            Time.deltaTime * positionSmoothing
        );

        transform.rotation = Quaternion.Euler(pitch, yaw, 0f);
    }
}