using UnityEngine;

//도마 코드 입니다.

public class CuttingBoard : NonPickable   
{
    [Header("도마 설정")]
    public float cutDistance = 2.5f;
    public CookingTimer cookingTimer;

    public override bool CanPlace(Pickable item) => item is Ingredient;

    void Awake()
    {
        if (cookingTimer != null)
        {
            cookingTimer.onTimerFinished.AddListener(OnCutFinished);
        }
    }

    // 상호작용1 : 들고 있으면 도마 위에 올리기, 비어있으면 집기
    public override void Interact(Player player)
    {
        if (player.heldItem != null)
        {
            if (TryPlaceItem(player.heldItem as Pickable))
            {
                player.heldItem = null;
                cookingTimer?.StartCooking();
            }
        }
        else if (heldItem != null)
        {
            cookingTimer?.StopCooking();
            player.heldItem = TakeItem(player);
        }
    }

    // 상호작용2: 플레이어 거리 체크해서 타이머 제어
    public override void InteractSecondary(Player player)
    {
        if (cookingTimer == null || heldItem == null) return;

        float dist = Vector3.Distance(player.GetPosition(), transform.position);

        if (dist > cutDistance)
        {
            cookingTimer.StopCooking();
            Debug.Log("플레이어가 너무 멀어서 타이머 정지");
        }
        else
        {
            cookingTimer.StartCooking();
            Debug.Log("플레이어가 가까워서 타이머 재개");
        }
    }

    // 타이머 완료 시 호출
    void OnCutFinished()
    {
        if (heldItem is Ingredient ingredient)
        {
            ingredient.isCut = true;
            Debug.Log($"{ingredient.name} 자르기 완료! 모델 변경 예정");
        }
    }
}
