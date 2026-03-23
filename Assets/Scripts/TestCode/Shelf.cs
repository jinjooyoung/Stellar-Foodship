using UnityEngine;

/* 테스트 코드 입니다.  Drop() 은 target 이 NonPickable 일 때만 작동하는데,
   테스트 할 NonPickable오브젝트가 씬에 없어 테스트합니당*/

public class Shelf : NonPickable
{
    // 상호작용 - 들고 있으면 내려놓기, 없으면 집기
    public override void Interact(Player player)
    {
        if (player.heldItem != null)
        {
            // 플레이어가 뭔가 들고 있으면 → 선반 위에 내려놓기
            player.Drop();
        }
        else if (heldItem != null)
        {
            // 플레이어가 비어있고 선반 위에 아이템 있으면 → 집기
            player.Pickup(heldItem as Pickable);
        }
    }

    // 모든 Pickable 수락, 슬롯이 비어있을 때만
    public override bool TryPlaceItem(Pickable item)
    {
        if (heldItem != null || item == null) return false;

        heldItem = item;

        Transform t = item.GetTransform();
        t.SetParent(holdPoint);
        t.localPosition = Vector3.zero;
        t.localRotation = Quaternion.identity;

        // 콜라이더 복구 (Pickup에서 껐던 것)
        Collider col = t.GetComponent<Collider>();
        if (col != null) col.enabled = true;

        return true;
    }
}