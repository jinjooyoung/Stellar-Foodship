using System.Collections;
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
    public Vector3 currentMoveDirection;   // 실제 이동 방향 (보간됨)
    public Vector3 lastInputDirection;     // 마지막 이동 방향
    public Vector2 inputVector;

    [Header("대쉬")]
    public Vector3 dashDirection;
    public float dashRemainingDistance;
    private bool isDashing; 
    public float characterRadius = 0.5f;
    public LayerMask dashObstacleLayer;
    public float lastDashTime; // 마지막 대시 시점 기록  
    public float dashCooldown = 1f; // 대시 쿨타임 (초)

    [Header("리스폰 / 사망")]
    public GameObject respawnPosition;
    public GameObject DiePosition;

    [Header("산소 시스템")]
    public float oxygen = 12f;
    public float maxOxygen = 12f;
    public OxygenUI oxygenUI;

    [HideInInspector] public bool isInOxygenZone = false;

    void Awake()
    {
        playerRigidbody = GetComponent<Rigidbody>();
        if (playerRigidbody == null)
        {
            Debug.LogWarning("플레이어 Rigidbody 없음. 생성됨");
            playerRigidbody = gameObject.AddComponent<Rigidbody>();
        }

        playerRigidbody.constraints = RigidbodyConstraints.FreezeRotation;
        playerRigidbody.interpolation = RigidbodyInterpolation.Interpolate;
        playerRigidbody.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;

    }

    /*void Update()
{
    if (heldItem  != null) Debug.Log($"플레이어 heldItem : {heldItem.ToString()}");
    if (target != null) Debug.Log($"플레이어 target : {target.ToString()}");

    targetUpdateTimer += Time.deltaTime;

    if (targetUpdateTimer >= targetUpdateInterval)
    {
        targetUpdateTimer = 0f;

        UpdateTarget();
    }

    SmoothMoveDirection();
    Rotate();
}*/
    // => 로그에 너무 많이 떠서 한번만 출력하도록 바꿔놨습니다. 
    // 주석 처리 한게 원본 Update코드입니다.

    /*private object lastHeldItem = null;
    private object lastTarget = null;*/

    void Update()
    {
        /*if (heldItem != lastHeldItem)
        {
            Debug.Log($"플레이어 heldItem : {heldItem?.ToString()}");
            lastHeldItem = heldItem;
        }
        if (target != lastTarget)
        {
            Debug.Log($"플레이어 target : {target?.ToString()}");
            lastTarget = target;
        }*/

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
        if(isDashing)
        {
            HandleDash();
            return;
        }
        if (state == PlayerState.Controllable)
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
        inputVector = input;

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
        if (target == null)
        {
            if (heldItem != null)
            {
                Drop();
            }
            return;
        }

        // 먼저 상호작용 시도
        bool interacted = false;

        // 접시
        if (target is Dish)
        {
            target.Interact(this);
            interacted = true;
        }
        // 쓰레기통
        else if (target is TrashCan)
        {
            target.Interact(this);
            interacted = true;
        }
        // 조리도구
        else if (target is Cookware)
        {
            target.Interact(this);
            interacted = true;
        }
        // 기타 논픽커블도 일단 시도
        else if (target is NonPickable)
        {
            target.Interact(this);
            interacted = true;
        }
        // 픽커블
        else if (target is Pickable)
        {
            target.Interact(this);
            interacted = true;
        }

        // 상호작용 실패 + 들고 있음 -> 드랍
        /*if (!interacted && heldItem != null)
        {
            Drop();
        }*/
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
        if (state != PlayerState.Controllable) return;
        if (Time.time < lastDashTime + dashCooldown) return;

        Vector3 dashDir;
        if (currentMoveDirection != Vector3.zero)
            dashDir = currentMoveDirection;
        else
            dashDir = lastInputDirection;

        if (dashDir == Vector3.zero)
            dashDir = transform.forward;

        dashDirection = dashDir.normalized;

        float maxDistance = dashDistance;
        RaycastHit hit;

        // --- BoxCast 설정 ---
        // 박스의 절반 크기 (Half Extents). 캐릭터 너비가 1m라면 0.5f를 넣습니다.
        Vector3 boxHalfSize = new Vector3(characterRadius, characterRadius, characterRadius);
        Vector3 rayStart = transform.position + (Vector3.down * 0.1f);

        // Physics.BoxCast(시작점, 절반크기, 방향, 결과, 회전, 최대거리, 레이어)
        if (Physics.BoxCast(rayStart, boxHalfSize, dashDirection, out hit, transform.rotation, dashDistance, dashObstacleLayer))
        {
            maxDistance = Mathf.Max(0, hit.distance - 0.3f);
        }

        isDashing = true;
        dashRemainingDistance = maxDistance;
        lastDashTime = Time.time;

        playerRigidbody.linearVelocity = new Vector3(0, playerRigidbody.linearVelocity.y, 0);
    }

    public void HandleDash()
    {
        if (!isDashing) return;

        float moveStep = dashSpeed * Time.fixedDeltaTime;

        if (moveStep > dashRemainingDistance)
        {
            moveStep = dashRemainingDistance;
        }

        Vector3 moveVector = dashDirection * moveStep;
        playerRigidbody.MovePosition(playerRigidbody.position + moveVector);

        dashRemainingDistance = dashRemainingDistance - moveStep;

        if (dashRemainingDistance <= 0)
        {
            isDashing = false;
            playerRigidbody.linearVelocity = Vector3.zero; // 대시 종료 시 딱 멈춤
        }
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

        if (heldItem == null) return;

        Pickable item = heldItem;
        heldItem = null;

        item.OnThrown(lastInputDirection, throwForce, this);
    }

    //=========================================================================

    // 위치 반환
    public Vector3 GetPosition()
    {
        return transform.position;
    }

    public void Die()
    {
        Debug.Log("플레이어 사망");
        this.transform.position = DiePosition.transform.position;

        state = PlayerState.Uncontrollable;
        oxygenUI.slider.gameObject.SetActive(false);


        StartCoroutine(Respawn());
    }

    public IEnumerator Respawn()
    {
        yield return new WaitForSeconds(5f);
        Debug.Log("플레이어 부활");
        this.transform.position = respawnPosition.transform.position; 
        
        state = PlayerState.Controllable;
        oxygenUI.slider.gameObject.SetActive(true);
        oxygen = maxOxygen;
    }

    //===============================산소 로직=======================================
    public void ChangeOxygen(float amount)
    {
        oxygen += amount;
        oxygen = Mathf.Clamp(oxygen, 0f, maxOxygen);

        if (oxygen <= 0f)
        {
            Die();
        }
    }

    //--------------------------------디버그용 기즈모--------------------------------
    // --- 스크립트의 멤버 변수 선언부 근처에 추가 ---
    [Header("Debug/Gizmos")]
    public bool showDashGizmos = true; // 기즈모를 켤지 끄는 스위치
    public Color gizmoColor = Color.cyan; // 기즈모 색상
    public float high;

    // --- 스크립트 하단에 함수 추가 ---}
    private void OnDrawGizmos()
    {
        // 1. 레이가 시작되는 지점 (rayStart와 동일한 로직)
        // 현재 코드 기준: 지하 10m
        Vector3 rayStart = transform.position + (Vector3.down * 0.1f);

        // 2. 박스의 크기 설정
        Vector3 boxHalfSize = new Vector3(characterRadius, characterRadius, characterRadius);
        Vector3 boxFullSize = boxHalfSize * 2f;

        // --- 기즈모 그리기 ---
        Gizmos.color = Color.yellow; // 시작점은 노란색

        // 플레이어의 현재 회전값을 반영하여 박스 그리기
        Matrix4x4 cubeMatrix = Matrix4x4.TRS(rayStart, transform.rotation, Vector3.one);
        Gizmos.matrix = cubeMatrix;
        Gizmos.DrawWireCube(Vector3.zero, boxFullSize);

        // --- 경로 그리기 ---
        Gizmos.matrix = Matrix4x4.identity; // 매트릭스 리셋
        Gizmos.color = Color.cyan;

        // 대시 방향이 결정되었다면 선으로 표시
        Vector3 dir = (dashDirection != Vector3.zero) ? dashDirection : transform.forward;
        Vector3 rayEnd = rayStart + (dir * dashDistance);
        Gizmos.DrawLine(rayStart, rayEnd);

        // 도착 지점 박스
        Matrix4x4 endMatrix = Matrix4x4.TRS(rayEnd, transform.rotation, Vector3.one);
        Gizmos.matrix = endMatrix;
        Gizmos.DrawWireCube(Vector3.zero, boxFullSize);
    }

}


