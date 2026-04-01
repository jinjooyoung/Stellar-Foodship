using UnityEditor.Rendering.LookDev;
using UnityEngine;

public enum PlayerState
{
    Uncontrollable,     // 조작 불가
    Controllable,       // 조작 가능
    IsAiming            // 던지기 에임 중
}

public class Player : MonoBehaviour
{
    [Header("참조 객체")]
    public InteractionFinder interactionFinder;
    public Transform holdPoint;

    [Header("상호작용 객체")]
    public Pickable heldItem;          // 들고 있는 아이템
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
        if (state == PlayerState.IsAiming)
        {
            playerRigidbody.linearVelocity = Vector3.zero;
            return;
        }
        else if (state == PlayerState.Controllable)
        {
            Move();
        }
    }

    // 탐색
    void UpdateTarget()
    {
        if (interactionFinder == null) return;

        target = interactionFinder.FindClosestInteractable();
       // Debug.Log($"타겟 오브젝트 : {target}");
    }

    //============================컨트롤러 호출(입력)============================

    // 이동 : WASD / Left Stick
    public void SetMoveInput(Vector2 input)
    {
        if (state == PlayerState.IsAiming || state == PlayerState.Uncontrollable)
            return;

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

    // 던지기 에이밍 : WASD / Left Stick
    public void Aiming(Vector2 input)
    {
        Vector3 dir = new Vector3(input.x, 0, input.y);

        if (dir.sqrMagnitude > 0.001f)
        {
            lastInputDirection = dir.normalized;
            currentMoveDirection = lastInputDirection;
        }
    }

    // 상호작용1 : J / Button South
    public void InteractPrimary()
    {
        Debug.Log($"{this.name} 플레이어 상호작용1 호출됨");

        // 들고 있는 아이템이 있으면 Drop
        if (heldItem != null)
        {
            Drop();
            return;
        }

        // 들고 있는 아이템이 없으면 target과 상호작용
        if (target == null) return;
        target.Interact(this);
    }

    // 상호작용2 : K / Button West
    public void StartInteractSecondary()
    {
        // 아이템을 들고있다면
        if (heldItem != null)
        {
            // 던지기 에임 시작
            // 나중에 플레이어에 Aimming 함수 작성 후 Aimming 호출
            state = PlayerState.IsAiming;

            playerRigidbody.linearVelocity = Vector3.zero;
            currentMoveDirection = Vector3.zero;
            targetMoveDirection = Vector3.zero;
        }

        Debug.Log($"{this.name} 플레이어 상호작용1 호출됨");
        if (target == null) return;

        // 굳이 타겟이 픽커블인지 논픽커블인지 구분할 필요 없을 것 같아서 주석.
        // 필요없는거 확실하면 그때가서 삭제하는걸로
        /*// target이 Nonpickable이면 NonPickable의 InteractSecondary 호출
        NonPickable nonPickable = (target as MonoBehaviour)?.GetComponent<NonPickable>();
        if (nonPickable != null)
        {
            nonPickable.InteractSecondary(this);
            return;
        }*/

        target.InteractSecondary(this);
    }

    public void EndSecondaryAction()
    {
        if (state != PlayerState.IsAiming || state == PlayerState.Uncontrollable) return;

        // 던지기
        Throw();

        state = PlayerState.Controllable;
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
        Vector3 lookDir;
        if (state == PlayerState.IsAiming)
        {
            // 에이밍 중 -> 즉각 반응
            lookDir = lastInputDirection;
        }
        else
        {
            // 평소 -> 부드러운 회전 유지
            lookDir = currentMoveDirection;
        }

        if (lookDir == Vector3.zero) return;

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
        if (target == null) return;

        target.Interact(this);  //Nonpickable이든 Pickable이든 Interact에 맡김
    }

    // 아이템 내려놓기
    public void Drop()
    {
        // 타겟 논픽커블 있으면 그 위에 올릴 수 있는지 체크해서 그 위에 올리거나 그냥 바닥에 드랍하거나 로직 추가해야함

        if (heldItem == null) return;

        // target이 NonPickable일 때만 올리기 시도
        if (target != null)
        {
            NonPickable nonPickable = (target as MonoBehaviour)?.GetComponent<NonPickable>();
            if (nonPickable != null)
            {
                nonPickable.Interact(this);
                if (heldItem == null) return;   //드랍 성공했으면 끝

            }
        }
        
        // NonPickable이 없거나 드랍 실패 -> 바닥 드랍
        Transform itemTransform = heldItem.GetTransform();
        itemTransform.SetParent(null);

        Rigidbody rb = itemTransform.GetComponent<Rigidbody>();
        Debug.Log($"rb null? {rb == null}");
        if (rb != null)
        {
            Debug.Log($"isKinematic before: {rb.isKinematic}, useGravity brfore : {rb.useGravity}");
            rb.useGravity = true;
            rb.isKinematic = false;
            rb.constraints = RigidbodyConstraints.None;     //constraints 해제추가. Pickup에서 reezeAll로 잠갔으니 Drop할 때 풀어줌
            Debug.Log($"isKinematic after: {rb.isKinematic}, useGravity after : {rb.useGravity}");
        }

        /* Pickup에서 col.enabled = false로 껐으니 Drop할 때 콜라이더도 다시 켜줘야함
             NonPickable.TryPlaceItem()에는 이미 col.enabled = true가 있는데, 바닥 드랍 케이스에는 없어서 추가*/
        Collider col = itemTransform.GetComponent<Collider>();
        if (col != null)
        {
            col.enabled = true;
        }

        heldItem = null;
        Debug.Log("바닥에 드랍");

        /*내려놓을 NonPickable이 주변에 없거나 NonPickable != null 라면 helditem을 player 하위에서 분리하고
        helditem=null + 물리 on -> 중력에 의해 그냥 바닥에 떨어지도록*/
    }

    // 던지기
    public void Throw()
    {
        Debug.Log("플레이어 Throw 호출");
        // 던지기 기능 구현
    }

    //=========================================================================

    // 위치 반환
    public Vector3 GetPosition()
    {
        return transform.position;
    }
}


    