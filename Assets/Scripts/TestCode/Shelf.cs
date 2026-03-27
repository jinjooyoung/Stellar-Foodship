using UnityEngine;

/* 테스트 코드 입니다.  Drop() 은 target 이 NonPickable 일 때만 작동하는데,
   테스트 할 NonPickable오브젝트가 씬에 없어 테스트합니당

 ㄴ> 코드 너무 좋은 것 같아요!!! 리팩토링만 함! 테스트 코드가 아니라 그냥 이대로
사용해도 될 듯 합니다!*/

public class Shelf : NonPickable
{
    public override bool CanPlace(Pickable item) => true;

    // 상호작용 - 들고 있으면 내려놓기, 없으면 집기
    public override void Interact(Player player)
    {
        if (player.heldItem != null)
        {
            // 플레이어가 뭔가 들고 있으면 → 선반 위에 내려놓기
            if (TryPlaceItem(player.heldItem as Pickable))
            {
                player.heldItem = null;
            }
        }
        else if (heldItem != null)
        {
            // 플레이어가 비어있고 선반 위에 아이템 있으면 → 집기
            player.heldItem = TakeItem(player);
        }
    }

    public override void InteractSecondary(Player player)
    {
        Debug.Log($"{name} Secondary Interact 없음");
    }

    // TryPlaceItem 선반 말고도 도마나 조리기 등에도 쓰여서 NonPickable 클래스의 virtual 함수로 넘김
}