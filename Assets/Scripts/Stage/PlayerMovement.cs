using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 5f;
    public float moveSmoothSpeed = 10f;

    [Header("Rotation")]
    public float rotateSpeed = 10f;

    private Vector2 moveInput;

    private Vector3 targetDirection;     // 입력 방향
    private Vector3 currentDirection;    // 보간된 방향

    private Rigidbody rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();

        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody>();
        }

        rb.useGravity = false;
        rb.constraints = RigidbodyConstraints.FreezeRotation;
        rb.interpolation = RigidbodyInterpolation.Interpolate;
    }

    // 인풋
    public void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();

        targetDirection = new Vector3(moveInput.x, 0f, moveInput.y).normalized;
    }

    void Update()
    {
        SmoothDirection();
        Rotate();
    }

    void FixedUpdate()
    {
        Move();
    }

    // ========================= 이동 =========================

    void SmoothDirection()
    {
        currentDirection = Vector3.Lerp(
            currentDirection,
            targetDirection,
            moveSmoothSpeed * Time.deltaTime
        );
    }

    void Move()
    {
        if (currentDirection.sqrMagnitude < 0.001f)
        {
            rb.linearVelocity = Vector3.zero;
            return;
        }

        Vector3 velocity = currentDirection * moveSpeed;
        velocity.y = 0f;

        rb.linearVelocity = velocity;
    }

    // ========================= 회전 =========================

    void Rotate()
    {
        if (currentDirection.sqrMagnitude < 0.001f) return;

        Quaternion targetRot = Quaternion.LookRotation(currentDirection);

        rb.rotation = Quaternion.Slerp(
            rb.rotation,
            targetRot,
            rotateSpeed * Time.deltaTime
        );
    }
}