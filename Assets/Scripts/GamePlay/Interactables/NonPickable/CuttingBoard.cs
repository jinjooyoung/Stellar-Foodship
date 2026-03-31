using UnityEngine;

public class CuttingBoard : NonPickable
{
    [Header("도마 설정")]
    public float playerDetectDist = 2.5f;   // 거리 감지 범위
    public float cutTime = 3f;              // 썰기 소요 시간
    public Timer timer;

    private Player currentInteractingPlayer;    // 현재 썰고 있는 플레이어

    void Awake()
    {
        if (timer == null)
        {
            Debug.LogWarning("도마 오브젝트에 타이머가 존재하지 않습니다!");
            return;
        }
        timer.OnCompleted += OnCutFinished;
    }

    //====================================Update====================================

    void Update()
    {
        // 썰고 있는 플레이어 없으면 패스
        if (currentInteractingPlayer == null) return;

        float dist = Vector3.Distance(currentInteractingPlayer.GetPosition(), transform.position);

        // 거리 멀어지면 타이머 정지
        if (dist > playerDetectDist)
        {
            timer.Stop();
            currentInteractingPlayer = null;
        }
    }

    //====================================Interact====================================

    // 상호작용1: 들고 있으면 도마 위에 올리기, 비어있으면 집기
    public override void Interact(Player player)
    {
        if (player.heldItem != null)
        {
            // 들고 있으면 도마 위에 내려놓기
            if (TryPlaceItem(player.heldItem as Pickable))
            {
                player.heldItem = null;
                SubscribeEvents();
            }
        }
        else if (heldItem != null)
        {
            // 도마 위에 아이템 있으면 집기
            if (heldItem is Ingredient ingredient)
            {
                // 타이머 있으면 체크
                if (timer != null)
                {
                    if (timer.CurrentTime <= 0f)
                    {
                        UnsubscribeEvents();
                        player.heldItem = TakeItem(player);
                    }
                }

                /*if (timer == null != timer.IsRunning)
                {
                    // 손질 중이면 못 집음
                    Debug.Log("손질 중인 재료이므로 들 수 없습니다");
                }
                else
                {
                    // 손질 끝났거나 안 했으면 집기 가능
                    UnsubscribeEvents();
                    player.heldItem = TakeItem(player);
                }*/
            }
            else
            {
                // 재료가 아니면 그냥 집기
                UnsubscribeEvents();
                player.heldItem = TakeItem(player);
            }
        }
    }

    // 상호작용2: 썰기 시작
    public override void InteractSecondary(Player player)
    {
        // 도마 위에 재료가 없거나 재료가 아니면
        if (!(heldItem is Ingredient ingredient))
        {
            Debug.Log("도마 위 오브젝트가 재료가 아닙니다.");
            return;
        }

        // 이미 썰린 재료면
        if (ingredient.isCut)
        {
            Debug.Log("이미 썰린 재료입니다.");
            return;
        }

        // 타이머 없으면
        if (timer == null)
        {
            Debug.LogWarning("도마 오브젝트에 타이머가 존재하지 않습니다!");
            return;
        }

        // 이미 손질 중이면
        if (timer.IsRunning)
        {
            Debug.Log("재료가 이미 손질 중입니다.");
            return;
        }

        // 상호작용 중인 플레이어 할당
        currentInteractingPlayer = player;

        // 남은 시간 0보다 크면 재개, 0이면 새로 시작
        if (timer.CurrentTime > 0f)
        {
            timer.Resume();
        }
        else
        {
            timer.StartTimer(cutTime);
        }
    }

    //====================================이벤트====================================

    // 썰기 완료 시 호출
    void OnCutFinished()
    {
        if (heldItem is Ingredient ingredient)
        {
            ingredient.OnCutComplete();
        }
        currentInteractingPlayer = null;
    }

    // 도마에 아이템 올릴 때 이벤트 구독
    void SubscribeEvents()
    {
        if (timer == null)
        {
            Debug.LogWarning("Timer가 연결되지 않았습니다!");
            return;
        }
        if (heldItem is Ingredient ingredient)
        {
            timer.OnCompleted += ingredient.OnCutComplete;
        }
    }

    // 도마에서 아이템 들 때 이벤트 해제
    void UnsubscribeEvents()
    {
        if (timer == null) return;
        if (heldItem is Ingredient ingredient)
        {
            timer.OnCompleted -= ingredient.OnCutComplete;
        }
    }
}