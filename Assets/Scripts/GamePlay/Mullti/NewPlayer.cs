using Fusion;
using System.Collections;
using UnityEngine;

public class NewPlayer : NetworkBehaviour
{
    [Header("State")]
    [Networked] public PlayerState State { get; set; }

    [Header("Interaction")]
    [SerializeField] private Transform holdPoint;
    [SerializeField] private float pickupDistance = 3.0f;
    [SerializeField] private float dropForce = 2.0f;
    [SerializeField] private LayerMask pickupMask;
    [SerializeField] private NewInteractionFinder interactionFinder;

    public Transform HoldPoint => holdPoint;

    [Networked] public NetworkObject HeldItem { get; set; }
    private INewInteractable target;
    public INewInteractable Target => target;

    public Vector3 HoldPointPos =>
        holdPoint != null ? holdPoint.position : transform.position + transform.forward * 1.2f + Vector3.up * 1.2f;

    [Header("Move")]
    public float moveSpeed = 3.8f;
    public float dashSpeed = 8f;
    public float dashTime = 0.3f;   // 대쉬가 정면 직선 이동이 아닌 순간 부스터 느낌임. 대쉬 지속 시간
    private NetworkCharacterController _ncc;

    [Header("Dash Cooldown")]
    [SerializeField] private float dashCooldown = 0.3f;

    [Header("Oxygen")]
    [Networked] public float Oxygen { get; set; }
    [Networked] public NetworkBool IsInOxygenZone { get; set; }
    public float MaxOxygen = 12f;
    [SerializeField]
    private float oxygenStartDelay = 3f;
    [Networked]
    private TickTimer RespawnTimer { get; set; }

    [Header("리스폰 / 사망")]
    [SerializeField]
    private Vector3 spawnPoint;
    [SerializeField]
    private Vector3 diePosition = new Vector3(0f, -50f, 0f);
    [SerializeField]
    private float respawnDelay = 5f;
    public bool isDead;

    [Networked] private TickTimer DashCooldownTimer { get; set; }

    private Vector3 targetMoveDir;
    private Vector3 currentMoveDir;
    private Vector3 lastInputDir;

    private float dashTimer;
    private bool isDashing;

    [Networked] private NetworkButtons PrevButtons { get; set; }

    private void Awake()
    {
        Application.runInBackground = true;
        Application.targetFrameRate = 120;

        if (interactionFinder == null) interactionFinder = GetComponent<NewInteractionFinder>();
        _ncc = GetComponent<NetworkCharacterController>();
    }

    public override void Spawned()
    {
        spawnPoint = transform.position;

        if (OxygenUIManager.Instance == null)
            return;

        int index =
            Object.InputAuthority.PlayerId - 1;

        Oxygen = MaxOxygen;

        OxygenUIManager.Instance.RegisterPlayer(
            this,
            index);
    }

    // ========================= 포톤 업데이트 =========================
    public override void FixedUpdateNetwork()
    {
        // 인풋이 없으면 리턴
        if (!GetInput<FusionBootstrap.NetworkInputData>(out var input))
            return;

        // 타겟 갱신
        target = interactionFinder.FindClosestInteractable();
        Debug.Log(target);

        // ================= 이동 =================

        Vector3 inputDir = new Vector3(input.move.x, 0, input.move.y);

        if (State == PlayerState.IsAiming)
        {
            if (inputDir.sqrMagnitude > 0.01f)
            {
                lastInputDir = inputDir.normalized;
                currentMoveDir = lastInputDir;
            }

            targetMoveDir = Vector3.zero;
        }
        else
        {
            SetMoveInput(inputDir);
        }

        // 보간
        SmoothMoveDirection();

        // 이동
        Move();

        // ================= 좌클릭 =================
        if (input.buttons.WasPressed(PrevButtons, (int)FusionBootstrap.InputButton.InteractPrimary))
        {
            /*if (!TryDropHeldBox())
                TryPickUp();*/
            if (target != null)
            {
                target.Interact(this);
            }
        }

        // ================= 우클릭 =================

        // 누르고 있는 동안
        /*if (input.buttons.IsSet((int)FusionBootstrap.InputButton.InteractSecondary))
        {
            if (HeldItem != null)
            {
                State = PlayerState.IsAiming;
            }
        }*/

        // 누른 순간
        if (input.buttons.WasPressed(PrevButtons, (int)FusionBootstrap.InputButton.InteractSecondary) && State == PlayerState.Controllable && State != PlayerState.IsAiming)
        {
            if (target != null)
            {
                target.InteractSecondary(this);
            }
            //InteractSecondary();
        }

        // 뗀 순간
        if (input.buttons.WasReleased(PrevButtons, (int)FusionBootstrap.InputButton.InteractSecondary))
        {
            if (State == PlayerState.IsAiming)
            {
                //Throw();
                State = PlayerState.Controllable;
            }
        }

        // ================= 대쉬 =================
        if (input.buttons.WasPressed(PrevButtons, (int)FusionBootstrap.InputButton.Dash))
        {
            StartDash();
        }

        HandleDash();

        PrevButtons = input.buttons;

        //================= 산소 =================

        if (!Object.HasStateAuthority)
            return;

        if (oxygenStartDelay > 0f)
        {
            oxygenStartDelay -= Runner.DeltaTime;

            if (oxygenStartDelay <= 0f)
                oxygenStartDelay = 0f;

            return;
        }

        float delta = Runner.DeltaTime;

        if (IsInOxygenZone)
        {
            Oxygen += 24f * delta;
            Debug.Log("산소증가 플레이어스크립트");
        }
        else
        {
            Oxygen -= 1f * delta;
            Debug.Log("산소감소 플레이어스크립트");
        }

        Oxygen = Mathf.Clamp(Oxygen, 0f, MaxOxygen);

        if (Oxygen <= 0f && State != PlayerState.Uncontrollable)
        {
            Die();
        }

        if (Object.HasStateAuthority &&
    isDead &&
    RespawnTimer.Expired(Runner))
        {
            Respawn();
        }
    }

    // ================= 이동 =================

    void SetMoveInput(Vector3 dir)
    {
        if (State == PlayerState.Uncontrollable) return;

        if (dir.sqrMagnitude > 0.001f)
        {
            lastInputDir = dir.normalized;
            targetMoveDir = lastInputDir;
        }
        else
        {
            targetMoveDir = Vector3.zero;
        }
    }

    void SmoothMoveDirection()
    {
        if (targetMoveDir != Vector3.zero)
        {
            currentMoveDir = Vector3.Lerp(
                currentMoveDir,
                targetMoveDir,
                10f * Runner.DeltaTime
            );
        }
        else
        {
            currentMoveDir = Vector3.zero;
        }
    }

    void Move()
    {
        _ncc.maxSpeed = isDashing ? dashSpeed : moveSpeed;
        
        if (_ncc != null)
        {
            // NCC에게 이동 명령 전달
            _ncc.Move(currentMoveDir);
        }

        // 회전 부드럽게
        Quaternion targetRot = Quaternion.LookRotation(lastInputDir);
        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            targetRot,
            10f * Runner.DeltaTime
        );
    }

    // ================= 대쉬 =================
    void StartDash()
    {
        // 이미 대쉬 중이면 무시
        if (isDashing) return;

        // 쿨타임 남아있으면 무시
        if (!DashCooldownTimer.ExpiredOrNotRunning(Runner))
            return;

        isDashing = true;
        dashTimer = dashTime;

        // 쿨타임 시작
        DashCooldownTimer = TickTimer.CreateFromSeconds(Runner, dashCooldown);
    }

    void HandleDash()
    {
        if (!isDashing) return;

        dashTimer -= Runner.DeltaTime;

        if (dashTimer <= 0f)
        {
            isDashing = false;
        }
    }

    public void SetHeldItem(NetworkObject obj)
    {
        HeldItem = obj;
    }

    // ================= 상호작용 =================

    void TryPickUp()
    {
        if (!Object.HasStateAuthority) return;

        if (HeldItem != null) return;

        Vector3 origin = transform.position;
        Vector3 direction = transform.forward;

        Debug.DrawRay(origin, direction * pickupDistance, Color.red, 3f);

        if (Physics.Raycast(origin, direction, out RaycastHit hit, pickupDistance, pickupMask))
        {
            Debug.Log("집기 히트 됨");
            Debug.Log($"{Object}");

            NewPickable pickable = hit.collider.GetComponentInChildren<NewPickable>();
            if (pickable == null) return;

            pickable.PickUp(Object.InputAuthority);
            HeldItem = pickable.Object;
        }
        else
        {
            Debug.Log("집기 히트 안 됨");
        }
    }

    private bool TryDropHeldBox()
    {
        if (!Object.HasStateAuthority) return false;

        if (HeldItem == null) return false;

        Debug.Log("TryDropHeldBox 호출됨");

        NewPickable pickable = HeldItem.GetComponent<NewPickable>();
        Debug.Log($"pickable : {pickable}");
        if (pickable == null) return false;

        pickable.Drop(transform.forward * dropForce);
        HeldItem = null;
        return true;
    }

    /*void InteractPrimary()
    {
        if (target == null)
        {
            if (heldItem != null)
                Drop();
            return;
        }

        Debug.Log($"{target} 상호작용1 호출");
    }

    void InteractSecondary()
    {
        Debug.Log($"{target} 상호작용2 호출");
    }

    void Drop()
    {
        if (heldItem == null) return;

        heldItem.transform.SetParent(null);
        heldItem = null;
    }

    void Throw()
    {
        if (heldItem == null) return;

        Pickable item = heldItem;
        heldItem = null;

        Debug.Log($"{item} 던지기 호출");
    }*/

    void Die()
    {
        Debug.Log("Die 호출");

        if (isDead)
            return;

        isDead = true;

        Debug.Log("플레이어 사망");

        transform.position = diePosition;

        State = PlayerState.Uncontrollable;

        RespawnTimer =
        TickTimer.CreateFromSeconds(
            Runner,
            respawnDelay);
    }

    void Respawn()
    {
        transform.position = spawnPoint;

        Oxygen = MaxOxygen;

        State = PlayerState.Controllable;

        isDead = false;
    }

    IEnumerator RespawnRoutine()
    {
        Debug.Log("RespawnRoutine 시작");

        yield return new WaitForSeconds(
            respawnDelay);

        Debug.Log("리스폰 실행");

        transform.position = spawnPoint;

        Oxygen = MaxOxygen;

        State = PlayerState.Controllable;

        isDead = false;
    }
}