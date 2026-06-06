using UnityEngine;

public class ParkourController : MonoBehaviour
{
    [Header("Movement")]
    public float maxSpeed = 6f;
    public float acceleration = 25f;
    public float airAcceleration = 10f;
    public float friction = 15f;
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
    public float dashForce = 15f;
    public float dashTime = 0.2f;
    public float dashCooldown = 1f;
    public float dashInertia = 0.5f;

    private bool canDash = true;
    private bool isDashing;

    [Header("Knockback")]
    public float knockbackDeceleration = 5f;

    private bool hasKnockback;

    [Header("Camera")]
    public Transform cameraTransform;
    public float mouseSensitivity = 2f;
    public bool canLook = true;

    [Header("Interaction Lock")]
    public bool canMove = true;

    private float xRotation;
    private CharacterController controller;
    private Vector3 velocity;
    private Vector3 moveInput;
    private Vector3 currentMove;
    private bool wasGrounded;
    private Camera cachedCamera;

    public event System.Action OnJumped;
    public event System.Action OnDashStarted;

    public bool IsGrounded => controller.isGrounded;
    public bool IsMoving => moveInput.magnitude > 0.01f;
    void Start()
    {
        controller = GetComponent<CharacterController>();
        cachedCamera = Camera.main;
        if (cameraTransform == null) cameraTransform = cachedCamera.transform;
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        bool grounded = controller.isGrounded;

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
            if (hasKnockback)
            {
                currentMove = Vector3.Lerp(currentMove, moveInput * maxSpeed, knockbackDeceleration * Time.deltaTime);
                if (currentMove.magnitude < 0.1f)
                    hasKnockback = false;
            }
            else
            {
                Vector3 target = moveInput * maxSpeed;
                float accel = grounded ? acceleration : airAcceleration;
                currentMove = Vector3.Lerp(currentMove, target, accel * Time.deltaTime);

                if (moveInput.magnitude < 0.01f && grounded)
                    currentMove = Vector3.Lerp(currentMove, Vector3.zero, friction * Time.deltaTime);
            }

            controller.Move(currentMove * Time.deltaTime);
        }

        if (grounded && velocity.y < 0)
        {
            velocity.y = -2f;
            if (!wasGrounded) jumpCount = 0;
        }

        wasGrounded = grounded;

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }

    void HandleJump()
    {
        if (!canMove) return;

        if (!controller.isGrounded && jumpCount == 0)
            jumpCount = 1;

        if (jumpBufferTimer > 0 && jumpCount < maxJumps)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
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

        float x = Input.GetAxisRaw("Horizontal");
        float z = Input.GetAxisRaw("Vertical");

        Transform cam = cameraTransform ?? cachedCamera.transform;
        Vector3 forward = cam.forward;
        Vector3 right = cam.right;
        forward.y = 0f;
        right.y = 0f;
        forward.Normalize();
        right.Normalize();

        Vector3 dashDir = forward * z + right * x;
        if (dashDir.magnitude < 0.1f)
            dashDir = currentMove.magnitude > 0.1f ? currentMove.normalized : forward;
        dashDir.Normalize();

        float startTime = Time.time;
        while (Time.time < startTime + dashTime)
        {
            currentMove = Vector3.Lerp(currentMove, dashDir * dashForce, 12f * Time.deltaTime);
            controller.Move(currentMove * Time.deltaTime);
            yield return null;
        }

        currentMove = dashDir * dashForce * dashInertia;
        isDashing = false;
        yield return new WaitForSeconds(dashCooldown);
        canDash = true;
    }

    public bool EstaDashando => isDashing;

    public void ApplyKnockback(Vector3 force)
    {
        currentMove = new Vector3(force.x, 0f, force.z);
        velocity.y = force.y;
        hasKnockback = true;
    }
}
