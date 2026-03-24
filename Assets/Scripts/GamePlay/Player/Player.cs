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

    [Header("플레이어 물리")]
    public Rigidbody playerRigidbody;

    [Header("던지기")]
    public float throwForce = 5f;

    [Header("인풋 타입")]
    public PlayerInputType inputType;

    [Header("탐색 주기")]
    [SerializeField] private float targetUpdateInterval = 0.3f; // 탐색 주기 (초)
    private float targetUpdateTimer = 0f;

    [Header("개발 중 확인용 플레이어 상태")]        // 나중엔 헤더 지우고 NonSerialized로 변경
    [SerializeField] public PlayerState state;

    private Vector3 targetMoveDirection;    // 입력 방향
    private Vector3 currentMoveDirection;   // 실제 이동 방향 (보간됨)
    private Vector3 lastInputDirection;     // 마지막 이동 방향

    private bool isDashing;

    void Awake()
    {
        playerRigidbody = GetComponent<Rigidbody>();
        if (playerRigidbody == null)
        {
            Debug.LogWarning("플레이어 Rigidbody 없음. 생성됨");
            playerRigidbody = gameObject.AddComponent<Rigidbody>();
        }

        playerRigidbody.constraints = RigidbodyConstraints.FreezeRotation;
        playerRigidbody.useGravity = false;
        playerRigidbody.interpolation = RigidbodyInterpolation.Interpolate;
        playerRigidbody.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
    }

    void Update()
    {
        if (heldItem  != null) Debug.Log($"플레이어 heldItem : {heldItem.ToString()}");

        targetUpdateTimer += Time.deltaTime;

        if (targetUpdateTimer >= targetUpdateInterval)
        {
            targetUpdateTimer = 0f;

            UpdateTarget();
        }

        SmoothMoveDirection();
        Rotate();
    }

    private void FixedUpdate()
    {
        Move();
    }

    // 탐색
    void UpdateTarget()
    {
        if (interactionFinder == null) return;

        target = interactionFinder.FindClosestInteractable();
        Debug.Log($"타겟 오브젝트 : {target}");
    }

    //============================컨트롤러 호출(입력)============================

    // 이동 : WASD / Left Stick
    public void SetMoveInput(Vector2 input)
    {
        Vector3 dir = new Vector3(input.x, 0, input.y);

        if (dir.sqrMagnitude > 0.001f)
        {
            lastInputDirection = dir.normalized;
            targetMoveDirection = lastInputDirection;
        }
        else
        {
            targetMoveDirection = Vector3.zero;
        }
    }

    // 상호작용1 : J / Button South
    public void InteractPrimary()
    {
        if (target == null) return;

        target.Interact(this);
    }

    // 상호작용2 : K / Button West
    public void InteractSecondary()
    {
        Debug.Log($"{this.name} 플레이어 상호작용1 호출됨");
        if (target == null) return;

        target.InteractSecondary(this);
    }

    // 대쉬 : Space / Button East
    public void Dash()
    {
        if (isDashing) return;

        Debug.Log("Dash 실행");
    }

    //====================================이동====================================

    // 방향 보간
    void SmoothMoveDirection()
    {
        if (targetMoveDirection != Vector3.zero)
        {
            currentMoveDirection = Vector3.Lerp(
                currentMoveDirection,
                targetMoveDirection,
                moveSmoothSpeed * Time.deltaTime
            );
        }
        else
        {
            currentMoveDirection = Vector3.zero;
        }
    }

    // 이동
    void Move()
    {
        if (isDashing || currentMoveDirection == Vector3.zero)
        {
            playerRigidbody.linearVelocity = Vector3.zero;
            return;
        }

        Vector3 desiredVelocity = currentMoveDirection * moveSpeed;
        desiredVelocity.y = 0f;
        playerRigidbody.linearVelocity = desiredVelocity;
    }

    // 회전 보간
    void Rotate()
    {
        if (currentMoveDirection == Vector3.zero) return;

        Quaternion targetRotation = Quaternion.LookRotation(lastInputDirection);
        playerRigidbody.rotation = Quaternion.Slerp(
            playerRigidbody.rotation,
            targetRotation,
            moveSmoothSpeed * Time.deltaTime
        );
    }

    //====================================행동==========================================

    // 아이템 집기
    public void Pickup()
    {
        if (heldItem != null) return;
        Debug.Log("플레이어 픽업 호출");
        if (target is Pickable pickable)
        {
            Debug.Log("플레이어 픽업 타겟이 픽커블");
            if (pickable.TryPickUp(this))
            {
                heldItem = pickable;
            }
        }
        else if (target != null)
        {
            Debug.Log("플레이어 픽업 타겟이 논픽커블");
            NonPickable nonPickable = target as NonPickable;
            if (nonPickable != null)
            {
                heldItem = nonPickable.TakeItem(this);
            }
        }
    }

    // 아이템 내려놓기
    public void Drop()
    {
        // 타겟 논픽커블 있으면 그 위에 올릴 수 있는지 체크해서 그 위에 올리거나 그냥 바닥에 드랍하거나 로직 추가해야함

        if (heldItem == null) return;

        //타겟이 NonPickable이고 아이템을 받을 수 있으면 그 위에 올림
        if (target != null)
        {
            Debug.Log("Drop 호출 타겟 Null 아님");
            NonPickable nonPickable = (target as MonoBehaviour)?.GetComponent<NonPickable>();
            if (nonPickable != null && nonPickable.CanPlace(heldItem as Pickable) && nonPickable.TryPlaceItem(heldItem as Pickable))
            {
                heldItem = null;
                return;
            }

        }

        Transform itemTransform = heldItem.GetTransform();
        itemTransform.SetParent(null);

        heldItem = null;

        // 올릴 곳 없으면 드랍 취소
        Debug.Log("내려놓을 곳 없음. 드랍 취소");

        /*내려놓을 NonPickable이 주변에 없거나 NonPickable != null 라면 helditem을 player 하위에서 분리하고
        helditem=null + 물리 on -> 중력에 의해 그냥 바닥에 떨어지도록*/
    }

    // 던지기
    public void Throw()
    {
        if (heldItem == null) return;

        Transform itemTransform = heldItem.GetTransform();
        itemTransform.SetParent(null);

        // Throw 시스템 만들어야할듯 물리로 던지기 싫음!!
        Debug.Log("Throw 실행");

        heldItem = null;
    }

    //=========================================================================

    // 위치 반환
    public Vector3 GetPosition()
    {
        return transform.position;
    }
}