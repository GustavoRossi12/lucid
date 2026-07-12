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

    enum State { Idle, Walk, Jump, Dash }
    enum Direction { Front, Back, Left, Right }

    State currentState;
    Direction currentDirection;

    public bool onCeiling;

    float timer;
    int currentFrame;
    Vector3 lastMovementDirection;
    Vector3 inputDirection;

    void Update()
    {
        Billboard();

        Vector2 input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        inputDirection = new Vector3(input.x, 0f, input.y).normalized;

        State previousState = currentState;
        Direction previousDirection = currentDirection;

        DetectState(input);
        DetectDirection();

        if (currentState != previousState || currentDirection != previousDirection)
        {
            currentFrame = 0;
            timer = 0f;
            Sprite[] anim = GetCurrentAnimation();
            if (anim != null && anim.Length > 0)
                spriteRenderer.sprite = anim[0];
        }

        Animate();
    }

    void Billboard()
    {
        Vector3 lookDir = cameraTransform.forward;
        lookDir.y = 0f;
        transform.rotation = Quaternion.LookRotation(lookDir);
        if (onCeiling)
            transform.rotation *= Quaternion.Euler(0f, 0f, 180f);
    }

    void DetectDirection()
    {
        Vector3 camForward = cameraTransform.forward;
        camForward.y = 0f;
        camForward.Normalize();

        Vector3 camRight = cameraTransform.right;
        camRight.y = 0f;
        camRight.Normalize();

        Vector3 worldMove = camForward * inputDirection.z + camRight * inputDirection.x;

        if (worldMove.magnitude > 0.01f)
            lastMovementDirection = worldMove.normalized;

        Vector3 dirToEvaluate = currentState == State.Idle
            ? (lastMovementDirection.magnitude > 0.01f ? lastMovementDirection : camForward)
            : (worldMove.magnitude > 0.01f ? worldMove.normalized
                : (lastMovementDirection.magnitude > 0.01f ? lastMovementDirection : camForward));

        float forwardDot = Vector3.Dot(dirToEvaluate, camForward);
        float rightDot = Vector3.Dot(dirToEvaluate, camRight);

        if (Mathf.Abs(forwardDot) >= Mathf.Abs(rightDot))
            currentDirection = forwardDot >= 0 ? Direction.Back : Direction.Front;
        else
            currentDirection = rightDot >= 0 ? Direction.Right : Direction.Left;
    }

    void DetectState(Vector2 input)
    {
        bool moving = input.magnitude > 0.1f;
        bool grounded = controller.isGrounded || onCeiling;
        bool dashing = Input.GetKey(KeyCode.LeftShift);

        if (dashing) currentState = State.Dash;
        else if (!grounded) currentState = State.Jump;
        else if (moving) currentState = State.Walk;
        else currentState = State.Idle;
    }

    void Animate()
    {
        Sprite[] anim = GetCurrentAnimation();
        if (anim == null || anim.Length == 0) return;

        timer += Time.deltaTime;
        if (timer >= 1f / animationFPS)
        {
            timer = 0f;
            currentFrame++;
            if (currentFrame >= anim.Length) currentFrame = 0;
            spriteRenderer.sprite = anim[currentFrame];
        }
    }

    Sprite[] GetCurrentAnimation()
    {
        switch (currentState)
        {
            case State.Idle: return GetDirectionalAnimation(idleFront, idleBack, idleLeft, idleRight);
            case State.Walk: return GetDirectionalAnimation(walkFront, walkBack, walkLeft, walkRight);
            case State.Jump: return GetDirectionalAnimation(jumpFront, jumpBack, jumpLeft, jumpRight);
            case State.Dash: return GetDirectionalAnimation(dashFront, dashBack, dashLeft, dashRight);
        }
        return idleFront;
    }

    Sprite[] GetDirectionalAnimation(Sprite[] front, Sprite[] back, Sprite[] left, Sprite[] right)
    {
        switch (currentDirection)
        {
            case Direction.Front: return front;
            case Direction.Back: return back;
            case Direction.Left: return onCeiling ? right : left;
            case Direction.Right: return onCeiling ? left : right;
        }
        return front;
    }
}
