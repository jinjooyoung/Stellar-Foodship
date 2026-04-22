using UnityEngine;
using DG.Tweening;

public class PlayerAnimationController : MonoBehaviour
{
    [Header("References")]
    public Player player;
    public Transform bottomTransform;

    [Header("Move Lean Settings")]
    public float maxTiltAngle = 20f;        // 이동 시 뒤로 기울어지는 최대 각도
    public float leanInDuration = 0.5f;     // 최대 각도까지 도달하는 시간
    public Ease leanInEase = Ease.InQuad;   // 서서히 가속도가 붙는 느낌

    [Header("Stop Bounce Settings")]
    public float bounce1 = 30f;             // 1단계: 앞으로 확 튕김
    public float bounce2 = -15f;            // 2단계: 다시 뒤로 반동
    public float bounce3 = 5f;              // 3단계: 앞으로 살짝
    public float bounceDuration = 0.15f;    // 각 반동 사이의 시간

    [Header("Idle Settings")]
    public float idleStretch = 1.05f;
    public float idleDuration = 0.6f;

    private Sequence activeSequence;
    private bool isMoving;
    private bool wasMoving;

    void Start()
    {
        PlayIdleAnimation();
    }

    void Update()
    {
        HandleAnimationState();
    }

    private void HandleAnimationState()
    {
        // 인풋벡터있으면 이동중
        isMoving = player.inputVector.magnitude > 0.1f;

        if (isMoving && !wasMoving)
        {
            OnStartMove();
        }
        else if (!isMoving && wasMoving)
        {
            OnStopMove();
        }

        wasMoving = isMoving;
    }

    private void OnStartMove()
    {
        activeSequence?.Kill();
        activeSequence = DOTween.Sequence();

        // X축 로컬 회전값을 음수 방향으로 서서히 가속하며 기울임
        activeSequence.Append(bottomTransform.DOLocalRotate(new Vector3(-maxTiltAngle, 0, 0), leanInDuration).SetEase(leanInEase));

        // 이동 중에는 Idle 스케일이 꼬이지 않게 1로 초기화
        bottomTransform.DOScale(Vector3.one, 0.2f);
    }

    private void OnStopMove()
    {
        activeSequence?.Kill();
        activeSequence = DOTween.Sequence();

        // 멈췄을 때 X축 반동: bounce1 -> 2 -> 3 -> 0
        activeSequence.Append(bottomTransform.DOLocalRotate(new Vector3(bounce1, 0, 0), bounceDuration).SetEase(Ease.OutQuad))
                      .Append(bottomTransform.DOLocalRotate(new Vector3(bounce2, 0, 0), bounceDuration).SetEase(Ease.InOutQuad))
                      .Append(bottomTransform.DOLocalRotate(new Vector3(bounce3, 0, 0), bounceDuration).SetEase(Ease.InOutQuad))
                      .Append(bottomTransform.DOLocalRotate(Vector3.zero, bounceDuration).SetEase(Ease.OutBack))
                      .OnComplete(() => {
                          if (!isMoving) PlayIdleAnimation();
                      });
    }

    private void PlayIdleAnimation()
    {
        activeSequence?.Kill();
        activeSequence = DOTween.Sequence();

        // 위아래로 늘어나는 모션
        activeSequence.Append(bottomTransform.DOScaleY(idleStretch, idleDuration).SetEase(Ease.InOutQuad))
                      .Append(bottomTransform.DOScaleY(1f, idleDuration).SetEase(Ease.InOutQuad))
                      .SetLoops(-1);
    }
}