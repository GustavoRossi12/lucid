using UnityEngine;
public class CameraOrbit : MonoBehaviour
{
    [Header("Target")]
    public Transform target;
    [Header("Rotation")]
    public float mouseSensitivity = 3f;
    public float fixedPitch = 15f;
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
    [Header("Collision")]
    public LayerMask collisionMask;
    public float collisionRadius = 0.3f;
    private Camera cam;
    void Start()
    {
        cam = GetComponentInChildren<Camera>();
        Cursor.lockState = CursorLockMode.Locked;
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
        yaw += Input.GetAxis("Mouse X") * mouseSensitivity;
        yaw = Mathf.Repeat(yaw, 360f);
    }
    void HandleZoom()
    {
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
        Vector3 targetPosition = target.position + Vector3.up * heightOffset;
        Quaternion rotation = Quaternion.Euler(fixedPitch, yaw, 0f);
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

        // Rotação totalmente fixa — sem LookAt que quebrava o pitch
        transform.rotation = Quaternion.Euler(fixedPitch, yaw, 0f);
    }
}