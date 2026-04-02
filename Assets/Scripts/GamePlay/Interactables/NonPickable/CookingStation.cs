using UnityEngine;

public class CookingStation : NonPickable
{
    [Header("조리기 설정")]
    public StationType stationType;
    public float cookTime = 5f;

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

                        if (cookware.timer.CurrentTime > 0f)
                        {
                            cookware.timer.Resume();
                        }
                        else
                        {
                            cookware.timer.StartTimer(cookTime);
                        }
                    }
                }
            }
        }
        else
        {
            // 안 들고 있으면 집기
            player.heldItem = TakeItem(player);

            // 집은 아이템이 조리도구면 타이머 정지
            if (player.heldItem is Cookware cookware)
            {
                cookware.timer?.Stop();
            }
        }
    }

    public override void InteractSecondary(Player player)
    {
    }
}