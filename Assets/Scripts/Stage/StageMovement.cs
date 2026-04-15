using UnityEngine;
using UnityEngine.InputSystem;

public class StageMovement : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 5f;
    public float moveSmoothSpeed = 10f;

    [Header("Rotation")]
    public float rotateSpeed = 10f;

    // 우주선이 엉뚱한 방향을 본다면 이 값을 -90, 0, 90, 180 등으로 수정해 보세요!
    public float zRotationOffset = -90f;

    private Vector2 moveInput;

    private Vector3 targetDirection;
    private Vector3 currentDirection;

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

    public void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();

        if (moveInput.sqrMagnitude > 0.01f)
        {
            targetDirection = new Vector3(moveInput.x, moveInput.y, 0f).normalized;
        }
        else
        {
            targetDirection = Vector3.zero;
        }
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
        velocity.z = 0f;

        rb.linearVelocity = velocity;
    }

    // ========================= 회전 =========================

    void Rotate()
    {
        if (currentDirection.sqrMagnitude < 0.01f) return;

        // 회전을 계산할 전용 방향 벡터를 만듭니다.
        Vector3 rotDirection = currentDirection;

        // 💡 핵심 추가: 아래(S키, Y축 마이너스)로 향할 때, 회전 방향의 Y값을 양수(W키 방향)로 뒤집어줍니다!
        if (rotDirection.y < 0f)
        {
            rotDirection.y = Mathf.Abs(rotDirection.y);
        }

        // 1. 뒤집힌 방향을 기준으로 2D 각도(360도)를 구합니다.
        float angle = Mathf.Atan2(rotDirection.y, rotDirection.x) * Mathf.Rad2Deg;

        // 2. Z축에만 각도를 적용합니다.
        Quaternion targetRot = Quaternion.Euler(0f, 0f, angle + zRotationOffset);

        // 3. 부드럽게 회전 적용
        rb.rotation = Quaternion.Slerp(
            rb.rotation,
            targetRot,
            rotateSpeed * Time.deltaTime
        );
    }
}