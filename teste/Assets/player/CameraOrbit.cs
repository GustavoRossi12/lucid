using UnityEngine;
public class CameraOrbit : MonoBehaviour
{
    [Header("Target")]
    public Transform target;
    [Header("Rotation")]
    public float mouseSensitivity = 3f;
    public float minPitch = -10f;
    public float maxPitch = 50f;
    float yaw;
    float pitch;
    [Header("Zoom (Field of View)")]
    public float defaultFOV = 60f;
    public float minFOV = 20f;
    public float maxFOV = 90f;
    public float fovZoomSpeed = 10f;
    public float fovSmoothing = 8f;
    float _targetFOV;
    [Header("Distance")]
    public float distance = 5f;
    public float positionSmoothing = 10f;
    [Header("Height Control")]
    public float heightOffset = 1.5f;
    public float minHeightAboveTarget = 0.5f;
    [Header("Collision")]
    public LayerMask collisionMask;
    public float collisionRadius = 0.3f;
    Camera _cam;
    void Start()
    {
        _cam = GetComponentInChildren<Camera>();
        if (_cam == null)
            Debug.LogError("CameraOrbit: nenhuma Camera encontrada no objeto ou em seus filhos.", this);
        Cursor.lockState = CursorLockMode.Locked;
        yaw = transform.eulerAngles.y;
        pitch = NormalizeAngle(transform.eulerAngles.x);
        pitch = Mathf.Clamp(pitch, minPitch, maxPitch);
        _targetFOV = defaultFOV;
        _cam.fieldOfView = defaultFOV;
    }
    void LateUpdate()
    {
        if (target == null) return;
        HandleMouse();
        HandleFOVZoom();
        UpdateCameraPosition();
    }
    void HandleMouse()
    {
        yaw += Input.GetAxis("Mouse X") * mouseSensitivity;
        pitch -= Input.GetAxis("Mouse Y") * mouseSensitivity;
        yaw = Mathf.Repeat(yaw, 360f);
        pitch = Mathf.Clamp(pitch, minPitch, maxPitch);
    }
    void HandleFOVZoom()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        _targetFOV -= scroll * fovZoomSpeed;
        _targetFOV = Mathf.Clamp(_targetFOV, minFOV, maxFOV);
        _cam.fieldOfView = Mathf.Lerp(_cam.fieldOfView, _targetFOV,
                                       Time.deltaTime * fovSmoothing);
    }
    void UpdateCameraPosition()
    {
        Vector3 targetPosition = target.position + Vector3.up * heightOffset;
        Quaternion rotation = Quaternion.Euler(pitch, yaw, 0f);
        Vector3 desiredOffset = rotation * new Vector3(0f, 0f, -distance);
        Vector3 desiredPosition = targetPosition + desiredOffset;
        float minY = targetPosition.y + minHeightAboveTarget;
        if (desiredPosition.y < minY)
        {
            desiredPosition.y = minY;
            Vector3 correctedDir = desiredPosition - targetPosition;
            pitch = Mathf.Atan2(-correctedDir.y,
                                 new Vector2(correctedDir.x, correctedDir.z).magnitude)
                    * Mathf.Rad2Deg;
            pitch = Mathf.Clamp(pitch, minPitch, maxPitch);
        }
        Vector3 dir = (desiredPosition - targetPosition).normalized;
        float dist = Vector3.Distance(targetPosition, desiredPosition);
        if (Physics.SphereCast(targetPosition, collisionRadius, dir,
                               out RaycastHit hit, dist, collisionMask))
        {
            desiredPosition = hit.point + hit.normal * collisionRadius;
        }
        transform.position = positionSmoothing > 0f
            ? Vector3.Lerp(transform.position, desiredPosition,
                           Time.deltaTime * positionSmoothing)
            : desiredPosition;
        transform.rotation = Quaternion.LookRotation(targetPosition - transform.position);
    }
    static float NormalizeAngle(float angle)
    {
        if (angle > 180f) angle -= 360f;
        return angle;
    }
}