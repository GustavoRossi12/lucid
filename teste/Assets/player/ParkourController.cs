using UnityEngine;
public class ParkourController : MonoBehaviour
{
    [Header("Movement")]
    public float speed = 6f;
    public float gravity = -20f;
    [Header("Jump")]
    public float jumpHeight = 2f;
    public int maxJumps = 2;
    private int jumpCount;
    [Header("Dash")]
    public float dashForce = 15f;
    public float dashTime = 0.2f;
    public float dashCooldown = 1f;
    private bool canDash = true;
    private bool isDashing;
    [Header("Camera")]
    public Transform cameraTransform;
    public float mouseSensitivity = 2f;
    float xRotation = 0f;
    private CharacterController controller;
    private Vector3 velocity;
    private bool wasGrounded;
    void Start()
    {
        controller = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
    }
    void Update()
    {
        HandleCamera();
        HandleJump();      // lê o input antes de resetar o jumpCount
        HandleMovement();  // reseta jumpCount só depois
        HandleDash();
    }
    void HandleCamera()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -80f, 80f);
        cameraTransform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        transform.Rotate(Vector3.up * mouseX);
    }
    void HandleMovement()
    {
        bool grounded = controller.isGrounded;

        if (!isDashing)
        {
            float x = Input.GetAxis("Horizontal");
            float z = Input.GetAxis("Vertical");
            Vector3 forward = cameraTransform.forward;
            Vector3 right = cameraTransform.right;
            forward.y = 0; right.y = 0;
            forward.Normalize(); right.Normalize();
            Vector3 move = forward * z + right * x;
            controller.Move(move * speed * Time.deltaTime);
        }

        if (grounded && velocity.y < 0)
        {
            velocity.y = -2f;
            if (!wasGrounded)
                jumpCount = 0;
        }

        wasGrounded = grounded;  // sempre atualiza, mesmo durante dash

        if (!isDashing)
        {
            velocity.y += gravity * Time.deltaTime;
            controller.Move(velocity * Time.deltaTime);
        }
    }
    void HandleJump()
    {
        if (!controller.isGrounded && jumpCount == 0)
            jumpCount = 1;
        if (Input.GetButtonDown("Jump") && jumpCount < maxJumps)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            jumpCount++;
        }
    }
    void HandleDash()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift) && canDash)
        {
            StartCoroutine(Dash());
        }
    }
    System.Collections.IEnumerator Dash()
    {
        canDash = false;
        isDashing = true;
        float x = Input.GetAxisRaw("Horizontal");
        float z = Input.GetAxisRaw("Vertical");
        Vector3 forward = cameraTransform.forward;
        Vector3 right = cameraTransform.right;
        forward.y = 0;
        right.y = 0;
        forward.Normalize();
        right.Normalize();
        Vector3 dashDirection = forward * z + right * x;
        if (dashDirection.magnitude < 0.1f)
            dashDirection = forward;
        dashDirection.Normalize();
        float startTime = Time.time;
        while (Time.time < startTime + dashTime)
        {
            controller.Move(dashDirection * dashForce * Time.deltaTime);
            yield return null;
        }
        isDashing = false;
        yield return new WaitForSeconds(dashCooldown);
        canDash = true;
    }


}