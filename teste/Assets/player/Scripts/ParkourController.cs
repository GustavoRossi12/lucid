using UnityEngine;

public class ParkourController : MonoBehaviour
{
    [Header("Movement")]
    public float maxSpeed = 15f;
    public float acceleration = 10f;
    public float airAcceleration = 12f;
    public float friction = 8f;
    public float gravity = -20f;

    [Header("Jump")]
    public float jumpHeight = 2f;
    public int maxJumps = 2;
    public float coyoteTime = 0.1f;
    public float jumpBufferTime = 0.1f;

    private int jumpCount;
    private float coyoteTimer;
    private float jumpBufferTimer;

    [Header("Dash")]
    public float dashForce = 30f;
    public float dashTime = 0.2f;
    public float dashCooldown = 1f;
    public float dashInertia = 0.5f;

    private bool canDash = true;
    private bool isDashing;

    [Header("Camera")]
    public Transform cameraTransform;
    public float mouseSensitivity = 2f;
    public bool canLook = true;

    [Header("Interaction Lock")]
    public bool canMove = true;

    [Header("Ceiling")]
    public CameraOrbit cameraOrbit;
    public SpriteVisual spriteVisual;

    private float xRotation;
    private CharacterController controller;
    private Vector3 velocity;
    private Vector3 moveInput;
    private Vector3 currentMove;
    private bool wasGrounded;
    private Camera cachedCamera;

    public event System.Action OnJumped;
    public event System.Action OnDashStarted;

    public bool onCeiling { get; private set; }

    public bool IsGrounded => controller.isGrounded;
    public bool IsMoving => moveInput.magnitude > 0.01f;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        cachedCamera = Camera.main;
        if (cameraTransform == null) cameraTransform = cachedCamera.transform;
        if (cameraOrbit == null) cameraOrbit = FindAnyObjectByType<CameraOrbit>();
        if (spriteVisual == null) spriteVisual = GetComponentInChildren<SpriteVisual>();
        Cursor.lockState = CursorLockMode.Locked;
        mouseSensitivity = Mathf.Max(0.1f, PlayerPrefs.GetFloat("Sensitivity", mouseSensitivity));
    }

    public void SetCeiling(bool on)
    {
        if (on == onCeiling) return;
        onCeiling = on;

        if (cameraOrbit == null) cameraOrbit = FindAnyObjectByType<CameraOrbit>();
        if (cameraOrbit != null)
            cameraOrbit.invertOrbit = on;
        if (spriteVisual == null) spriteVisual = GetComponentInChildren<SpriteVisual>();
        if (spriteVisual != null)
            spriteVisual.onCeiling = on;
    }

    void Update()
    {
        bool grounded = controller.isGrounded || onCeiling;

        HandleTimers(grounded);
        HandleCamera();
        HandleInput();
        HandleJump();
        HandleMovement(grounded);
        HandleDash();
    }

    void HandleTimers(bool grounded)
    {
        if (grounded) coyoteTimer = coyoteTime;
        else coyoteTimer -= Time.deltaTime;

        if (Input.GetButtonDown("Jump")) jumpBufferTimer = jumpBufferTime;
        else jumpBufferTimer -= Time.deltaTime;
    }

    void HandleCamera()
    {
        if (!canLook) return;

        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -80f, 80f);
        cameraTransform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
    }

    void HandleInput()
    {
        float x = Input.GetAxisRaw("Horizontal");
        float z = Input.GetAxisRaw("Vertical");

        Vector3 forward = cachedCamera.transform.forward;
        Vector3 right = cachedCamera.transform.right;
        forward.y = 0f;
        right.y = 0f;
        forward.Normalize();
        right.Normalize();

        moveInput = (forward * z + right * x).normalized;
    }

    void HandleMovement(bool grounded)
    {
        if (!canMove) return;

        if (!isDashing)
        {
            Vector3 target = moveInput * maxSpeed;
            float accel = grounded ? acceleration : airAcceleration;
            currentMove = Vector3.Lerp(currentMove, target, accel * Time.deltaTime);

            if (moveInput.magnitude < 0.01f && grounded)
                currentMove = Vector3.Lerp(currentMove, Vector3.zero, friction * Time.deltaTime);

            controller.Move(currentMove * Time.deltaTime);
        }

        float effectiveGrav = onCeiling ? Mathf.Abs(gravity) : gravity;

        if (grounded)
        {
            bool towardSurface = (velocity.y < 0 && !onCeiling) || (velocity.y > 0 && onCeiling);
            if (towardSurface)
            {
                velocity.y = onCeiling ? 2f : -2f;
                if (!wasGrounded) jumpCount = 0;
            }
        }

        wasGrounded = grounded;

        velocity.y += effectiveGrav * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }

    void HandleJump()
    {
        if (!canMove) return;

        bool groundedJump = controller.isGrounded || onCeiling;
        if (!groundedJump && jumpCount == 0)
            jumpCount = 1;

        if (jumpBufferTimer > 0 && jumpCount < maxJumps)
        {
            if (onCeiling)
            {
                SetCeiling(false);
                velocity.y = -Mathf.Sqrt(jumpHeight * -2f * gravity);
            }
            else
            {
                velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            }
            jumpCount++;
            jumpBufferTimer = 0;
            coyoteTimer = 0;
            OnJumped?.Invoke();
        }
    }

    void HandleDash()
    {
        if (!canMove) return;
        if (Input.GetKeyDown(KeyCode.LeftShift) && canDash)
            StartCoroutine(DashRoutine());
    }

    System.Collections.IEnumerator DashRoutine()
    {
        canDash = false;
        isDashing = true;
        OnDashStarted?.Invoke();

        currentMove = moveInput.magnitude > 0.1f ? moveInput * maxSpeed : cachedCamera.transform.forward * maxSpeed;
        currentMove.y = 0f;

        float startTime = Time.time;
        while (Time.time < startTime + dashTime)
        {
            currentMove = Vector3.Lerp(currentMove, currentMove.normalized * dashForce, 20f * Time.deltaTime);
            controller.Move(currentMove * Time.deltaTime);
            yield return null;
        }

        currentMove *= dashInertia;
        isDashing = false;
        yield return new WaitForSeconds(dashCooldown);
        canDash = true;
    }

    public bool EstaDashando => isDashing;

    public void AddInertia(Vector3 force)
    {
        currentMove = new Vector3(force.x, 0f, force.z);
        velocity.y = force.y;
    }

    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        var block = hit.collider.GetComponentInParent<PushBlock>();
        if (block != null)
        {
            Vector3 pushDir = new Vector3(hit.moveDirection.x, 0f, hit.moveDirection.z);
            if (pushDir.magnitude > 0.1f)
                block.Push(pushDir.normalized);
        }
    }
}
