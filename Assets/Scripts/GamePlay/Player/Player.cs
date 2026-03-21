using UnityEngine;

public enum PlayerState
{
    Uncontrollable,     // 조작 불가
    Controllable,       // 조작 가능
    Interacting         // 상호작용 중
}

public class Player : MonoBehaviour
{
    [Header("참조 객체")]
    public InteractionFinder interactionFinder;
    public Transform holdPoint;

    [Header("상호작용 객체")]
    public IInteractable heldItem;          // 들고 있는 아이템
    public IInteractable target;            // 현재 타겟

    [Header("이동 변수")]
    public float moveSpeed = 5f;
    public float moveSmoothSpeed = 10f;     // 보간 속도
    public float dashSpeed = 10f;
    public float dashDistance = 3f;

    [Header("던지기")]
    public float throwForce = 5f;

    [Header("인풋 타입")]
    public PlayerInputType inputType;

    [Header("탐색 주기")]
    [SerializeField] private float targetUpdateInterval = 0.1f; // 탐색 주기 (초)
    private float targetUpdateTimer = 0f;

    [Header("개발 중 확인용 플레이어 상태")]        // 나중엔 헤더 지우고 NonSerialized로 변경
    [SerializeField] public PlayerState state;

    private Vector3 targetMoveDirection;    // 입력 방향
    private Vector3 currentMoveDirection;   // 실제 이동 방향 (보간됨)

    private bool isDashing;

    void Update()
    {
        targetUpdateTimer += Time.deltaTime;

        if (targetUpdateTimer >= targetUpdateInterval)
        {
            targetUpdateTimer = 0f;

            UpdateTarget();
        }

        SmoothMoveDirection();
        Move();
        Rotate();
    }

    // 타겟 갱신
    void UpdateTarget()
    {
        if (interactionFinder == null) return;

        target = interactionFinder.FindClosestInteractable();
    }

    // 입력 방향 설정 (Controller에서 호출)
    public void SetMoveInput(Vector2 input)
    {
        targetMoveDirection = new Vector3(input.x, 0, input.y).normalized;
    }

    // 방향 보간
    void SmoothMoveDirection()
    {
        currentMoveDirection = Vector3.Lerp(
            currentMoveDirection,
            targetMoveDirection,
            moveSmoothSpeed * Time.deltaTime
        );
    }

    // 이동
    void Move()
    {
        if (isDashing) return;

        transform.Translate(currentMoveDirection * moveSpeed * Time.deltaTime, Space.World);
    }

    // 회전 보간
    void Rotate()
    {
        if (currentMoveDirection == Vector3.zero) return;

        Quaternion targetRotation = Quaternion.LookRotation(currentMoveDirection);

        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            targetRotation,
            moveSmoothSpeed * Time.deltaTime
        );
    }

    // 대쉬
    public void Dash()
    {
        if (isDashing) return;

        Debug.Log("Dash 실행");
    }

    // 상호작용
    public void InteractPrimary()
    {
        //if (target == null) return;

        //target.Interact(this);
    }

    // 아이템 집기
    public void Pickup(Pickable item)
    {
        heldItem = item;

        Transform t = item.GetTransform();
        t.SetParent(holdPoint);
        t.localPosition = Vector3.zero;
    }

    void DropToGround()
    {
        Transform t = heldItem.GetTransform();
        t.SetParent(null);

        // Rigidbody 있으니까 자연스럽게 떨어짐
        heldItem = null;
    }

    // 아이템 내려놓기
    public void Drop()
    {
        if (heldItem == null) return;

        Transform itemTransform = heldItem.GetTransform();
        itemTransform.SetParent(null);

        heldItem = null;
    }

    // 던지기
    public void Throw()
    {
        if (heldItem == null) return;

        Transform itemTransform = heldItem.GetTransform();
        itemTransform.SetParent(null);

        // TODO: ThrowSystem으로 위임 추천
        Debug.Log("Throw 실행");

        heldItem = null;
    }

    // 위치 반환
    public Vector3 GetPosition()
    {
        return transform.position;
    }
}