using UnityEngine;

public class CookingStation : NonPickable
{
    [Header("조리기 설정")]
    public StationType stationType;
    public float cookTime = 5f;
    public Cookware currentCookware;

    public bool _canPlace;
    public override bool canPlace => _canPlace;

    private void Awake()
    {
        currentCookware = null;
    }

    public override void Interact(Player player)
    {
        if (player.heldItem != null)
        {
            // 올리기 시도
            if (TryPlaceItem(player.heldItem as Pickable))
            {
                player.heldItem = null;

                // 올린 오브젝트가 조리도구인지 확인
                if (heldItem is Cookware cookware)
                {
                    currentCookware = cookware;
                    SubscribeEvents();
                    // 조리도구 타입이랑 스테이션 타입이 맞으면
                    if (cookware.GetRequiredStation() == stationType)
                    {
                        if (cookware.timer == null)
                        {
                            Debug.Log("조리기구에 타이머가 존재하지 않습니다!");
                            return;
                        }

                        if (cookware.timer.IsRunning)
                        {
                            Debug.Log("이미 타이머가 진행중입니다.");
                            return;
                        }

                        if (cookware.isComplete)
                        {
                            Debug.Log("이미 조리가 완료되었습니다.");
                            return;
                        }

                        if(cookware.isBurnt)
                        {
                            Debug.Log("이미 타버렸습니다.");
                            return;
                        }
                        if(cookware.currentIngredientIds.Count == 0)
                        {
                            Debug.Log("재료가 들어있어야 조리가 가능합니다.");
                            return;
                        }

                        if (cookware.timer.CurrentTime > 0f)
                        {
                            cookware.timer.Resume();
                            SoundManager.instance.PlaySound("Cooking");
                        }
                        else
                        {
                            cookware.timer.StartTimer(cookTime);
                            SoundManager.instance.PlaySound("Cooking");
                        }
                    }
                }
            }
        }
        else
        {
            UnsubscribeEvents();
            // 안 들고 있으면 집기
            player.heldItem = TakeItem(player);

            // 집은 아이템이 조리도구면 타이머 정지
            if (player.heldItem is Cookware cookware)
            {
                currentCookware = null;
                cookware.timer?.Stop();
                SoundManager.instance.StopSound("Cooking");
            }
        }
    }

    // 조리기에 아이템 올릴 때 이벤트 구독
    void SubscribeEvents()
    {
        if (currentCookware == null) return;

        if (currentCookware.timer == null)
        {
            Debug.LogWarning("Timer가 연결되지 않았습니다!");
            return;
        }
        if (heldItem is Cookware cookware)
        {
            currentCookware.timer.OnCompleted += cookware.OnCookingComplete;
        }
    }

    // 조리기에서 아이템 들 때 이벤트 해제
    void UnsubscribeEvents()
    {
        if (currentCookware == null) return;

        if (currentCookware.timer == null) return;
        if (heldItem is Cookware cookware)
        {
            currentCookware.timer.OnCompleted -= cookware.OnCookingComplete;
        }
    }

    public override void InteractSecondary(Player player)
    {
    }
}