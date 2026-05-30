using UnityEngine;

public class SpriteVisual : MonoBehaviour
{
    [Header("References")]
    public SpriteRenderer spriteRenderer;
    public Transform cameraTransform;
    public CharacterController controller;

    [Header("Animation")]
    public float animationFPS = 10f;

    [Header("Idle")]
    public Sprite[] idleFront;
    public Sprite[] idleBack;
    public Sprite[] idleLeft;
    public Sprite[] idleRight;

    [Header("Walk")]
    public Sprite[] walkFront;
    public Sprite[] walkBack;
    public Sprite[] walkLeft;
    public Sprite[] walkRight;

    [Header("Jump")]
    public Sprite[] jumpFront;
    public Sprite[] jumpBack;
    public Sprite[] jumpLeft;
    public Sprite[] jumpRight;

    [Header("Dash")]
    public Sprite[] dashFront;
    public Sprite[] dashBack;
    public Sprite[] dashLeft;
    public Sprite[] dashRight;

    enum State
    {
        Idle,
        Walk,
        Jump,
        Dash
    }

    enum Direction
    {
        Front,
        Back,
        Left,
        Right
    }

    State currentState;
    Direction currentDirection;

    float timer;
    int currentFrame;

    Vector3 lastPosition;
    Vector3 lastMovementDirection;

    void Start()
    {
        lastPosition = transform.parent.position;
    }

    void Update()
    {
        Billboard();

        DetectState();

        DetectDirection();

        Animate();

        lastPosition = transform.parent.position;
    }

    void Billboard()
    {
        Vector3 lookDir = cameraTransform.forward;

        lookDir.y = 0f;

        transform.rotation =
            Quaternion.LookRotation(lookDir);
    }

    void DetectDirection()
    {
        Vector3 movement = (transform.parent.position - lastPosition);
        movement.y = 0f;

        Vector3 camForward = cameraTransform.forward;
        camForward.y = 0f;
        camForward.Normalize();

        Vector3 camRight = cameraTransform.right;
        camRight.y = 0f;
        camRight.Normalize();

        if (movement.magnitude > 0.01f)
        {
            lastMovementDirection = movement.normalized;
        }

        Vector3 dirToEvaluate;

        switch (currentState)
        {
            case State.Idle:
                dirToEvaluate = lastMovementDirection.magnitude > 0.01f
                    ? lastMovementDirection
                    : camForward;
                break;

            case State.Jump:
            case State.Dash:
                dirToEvaluate = movement.magnitude > 0.01f
                    ? movement.normalized
                    : (lastMovementDirection.magnitude > 0.01f
                        ? lastMovementDirection
                        : camForward);
                break;

            default:
                dirToEvaluate = lastMovementDirection.magnitude > 0.01f
                    ? lastMovementDirection
                    : camForward;
                break;
        }
        float forwardDot = Vector3.Dot(dirToEvaluate, camForward);
        float rightDot = Vector3.Dot(dirToEvaluate, camRight);

        if (Mathf.Abs(forwardDot) >= Mathf.Abs(rightDot))
        {
            currentDirection = forwardDot >= 0
                ? Direction.Back
                : Direction.Front;
        }
        else
        {
            currentDirection = rightDot >= 0
                ? Direction.Right
                : Direction.Left;
        }
    }

    void DetectState()
    {
        Vector3 movement =
            (transform.parent.position - lastPosition)
            / Time.deltaTime;

        bool moving = movement.magnitude > 0.1f;

        bool grounded = controller.isGrounded;

        bool dashing = Input.GetKey(KeyCode.LeftShift);

        if (dashing)
        {
            currentState = State.Dash;
        }
        else if (!grounded)
        {
            currentState = State.Jump;
        }
        else if (moving)
        {
            currentState = State.Walk;
        }
        else
        {
            currentState = State.Idle;
        }
    }

    void Animate()
    {
        Sprite[] anim = GetCurrentAnimation();

        if (anim == null || anim.Length == 0)
            return;

        timer += Time.deltaTime;

        if (timer >= 1f / animationFPS)
        {
            timer = 0f;

            currentFrame++;

            if (currentFrame >= anim.Length)
            {
                currentFrame = 0;
            }

            spriteRenderer.sprite =
                anim[currentFrame];
        }
    }

    Sprite[] GetCurrentAnimation()
    {
        switch (currentState)
        {
            case State.Idle:
                return GetDirectionalAnimation(
                    idleFront,
                    idleBack,
                    idleLeft,
                    idleRight);

            case State.Walk:
                return GetDirectionalAnimation(
                    walkFront,
                    walkBack,
                    walkLeft,
                    walkRight);

            case State.Jump:
                return GetDirectionalAnimation(
                    jumpFront,
                    jumpBack,
                    jumpLeft,
                    jumpRight);

            case State.Dash:
                return GetDirectionalAnimation(
                    dashFront,
                    dashBack,
                    dashLeft,
                    dashRight);
        }

        return idleFront;
    }

    Sprite[] GetDirectionalAnimation(
        Sprite[] front,
        Sprite[] back,
        Sprite[] left,
        Sprite[] right)
    {
        switch (currentDirection)
        {
            case Direction.Front:
                return front;

            case Direction.Back:
                return back;

            case Direction.Left:
                return left;

            case Direction.Right:
                return right;
        }

        return front;
    }
}