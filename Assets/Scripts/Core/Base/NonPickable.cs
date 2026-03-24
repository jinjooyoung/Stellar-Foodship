using UnityEngine;

public abstract class NonPickable : MonoBehaviour, IInteractable
{
    [Header("슬롯")]
    public IInteractable heldItem;
    public Transform holdPoint;

    // 원래는 게임 시작하면 위에 올려져 있는 템 없어서 null인데 지금은 개발 중이니까 주석처리해둠
    /*void Awake()
    {
        heldItem = null;
    }*/

    //==================================공통 기능======================================

    // 기본적으로는 논픽커블 위에 못 올림
    /*이 주석은 이해하시면 지워주세요!!
    선반, 재료상자, 도마, 조리기 위에는 올릴 수 있고 제출창구, 접시리필기 위에는 올릴 수 없음.
    그래서 기본적으로는 false 선언 후 드랍로직 상단에 if 타겟.CanPlace로 체크 후에 올리기
    논픽커블 상속 클래스에서 override로 선반은 => true, 도마는 return item is Ingredient 이런식으로
    올릴 픽커블이 무엇인지에 따라 또 올릴 수 있는지 없는지도 override*/
    public virtual bool CanPlace(Pickable item) => false;

    // 논픽커블 위에 아이템 올리기
    public virtual bool TryPlaceItem(Pickable item)
    {
        if (heldItem != null || item == null) return false;

        heldItem = item;
        Debug.Log($"{this.name} helditem {heldItem.ToString()}");

        Transform t = item.GetTransform();
        t.SetParent(holdPoint);
        t.localPosition = Vector3.zero;
        t.localRotation = Quaternion.identity;

        // 콜라이더 복구 (Pickup에서 껐던 것)
        Collider col = t.GetComponent<Collider>();
        if (col != null) col.enabled = true;
        Debug.Log("TryPlaceItem 호출");
        return true;
    }

    // 논픽커블 위에 있는 아이템 -> Player가 들기
    public virtual IInteractable TakeItem(Player player)
    {
        Debug.Log("논픽커블 테이크아이템 호출됨");
        if (heldItem == null)
        {
            Debug.Log("논픽커블 테이크아이템 > 헬드아이템 null 리턴됨");
            return null;
        }

        IInteractable item = heldItem;
        heldItem = null;

        //item.GetTransform().SetParent(null);
        item.GetTransform().SetParent(player.holdPoint);
        item.GetTransform().localPosition = Vector3.zero;
        item.GetTransform().localRotation = Quaternion.identity;

        // 콜라이더 복구 (TryPlaceItem에서 껐던 것)
        Collider col = item.GetTransform().GetComponent<Collider>();
        if (col != null) col.enabled = false;

        return item;
    }

    //==================================개별 기능======================================

    // 상호작용1 : J / Button South
    public abstract void Interact(Player player);
    // 상호작용2 : K / Button West
    public abstract void InteractSecondary(Player player);

    //=================================데이터 전달======================================

    public Transform GetTransform()
    {
        return transform;
    }
}
