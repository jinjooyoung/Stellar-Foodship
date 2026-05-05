using Fusion;
using UnityEngine;

public class NewPlayer : NetworkBehaviour
{
    [Header("State")]
    [Networked] public PlayerState State { get; set; }

    [Header("References")]
    public Transform holdPoint;

    [Header("Interaction")]
    public Pickable heldItem;
    public IInteractable target;

    [Header("Move")]
    public float moveSpeed = 5f;
    public float dashSpeed = 12f;
    public float dashTime = 0.3f;   // 대쉬가 정면 직선 이동이 아닌 순간 부스터 느낌임. 대쉬 지속 시간

    [Header("Dash Cooldown")]
    [SerializeField] private float dashCooldown = 0.3f;

    [Networked] private TickTimer DashCooldownTimer { get; set; }

    private Vector3 targetMoveDir;
    private Vector3 currentMoveDir;
    private Vector3 lastInputDir;

    private float dashTimer;
    private bool isDashing;

    [Networked] private NetworkButtons PrevButtons { get; set; }

    // ========================= 포톤 업데이트 =========================
    public override void FixedUpdateNetwork()
    {
        // 인풋이 없으면 리턴
        if (!GetInput<FusionBootstrap.NetworkInputData>(out var input))
            return;

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
            InteractPrimary();
        }

        // ================= 우클릭 =================

        // 누르고 있는 동안
        if (input.buttons.IsSet((int)FusionBootstrap.InputButton.InteractSecondary))
        {
            if (heldItem != null)
            {
                State = PlayerState.IsAiming;
            }
        }

        // 누른 순간
        if (input.buttons.WasPressed(PrevButtons, (int)FusionBootstrap.InputButton.InteractSecondary) && State == PlayerState.Controllable && State != PlayerState.IsAiming)
        {
            InteractSecondary();
        }

        // 뗀 순간
        if (input.buttons.WasReleased(PrevButtons, (int)FusionBootstrap.InputButton.InteractSecondary))
        {
            if (State == PlayerState.IsAiming)
            {
                Throw();
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
    }

    // ================= 이동 =================

    void SetMoveInput(Vector3 dir)
    {
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
        float speed = isDashing ? dashSpeed : moveSpeed;

        if (currentMoveDir == Vector3.zero)
            return;

        transform.position += currentMoveDir * speed * Runner.DeltaTime;

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

    // ================= 상호작용 =================
    void InteractPrimary()
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
    }
}
