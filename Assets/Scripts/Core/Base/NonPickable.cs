using UnityEngine;

public abstract class NonPickable : MonoBehaviour, IInteractable
{
    public IInteractable heldItem;
    public Transform holdPoint;

    /*void Awake()
    {
        heldItem = null;
    }*/

    //==================================공통 기능======================================

    public virtual bool CanPlace(Pickable item) => false;

    public virtual bool TryPlaceItem(Pickable item)
    {
        if (heldItem != null || item == null) return false;
        heldItem = item;
        Debug.Log($"{this.name} helditem {heldItem.ToString()}");
        Transform t = item.GetTransform();
        t.SetParent(holdPoint);
        t.localPosition = Vector3.zero;
        t.localRotation = Quaternion.identity;

        // 콜라이더 복구
        Collider col = t.GetComponent<Collider>();
        if (col != null) col.enabled = true;  

        // 논픽커블에 올려도 물리 비활성 유지
        /*// Rigidbody 복구
        Rigidbody rb = t.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.useGravity = true;
            rb.isKinematic = false;
            rb.constraints = RigidbodyConstraints.None;
        }*/

        Debug.Log("TryPlaceItem 호출");
        return true;
    }

    public virtual IInteractable TakeItem(Player player)
    {
        Debug.Log("논픽커블 테이크아이템 호출됨");
        if (heldItem == null)
        {
            Debug.Log("논픽커블 테이크아이템 > 헬드아이템 null 반환됨");
            return null;
        }

        IInteractable item = heldItem;
        heldItem = null;

        item.GetTransform().SetParent(player.holdPoint);
        item.GetTransform().localPosition = Vector3.zero;
        item.GetTransform().localRotation = Quaternion.identity;

        // 콜라이더 끄기
        Collider col = item.GetTransform().GetComponent<Collider>();
        if (col != null) col.enabled = false;

        // Rigidbody 끄기
        Rigidbody rb = item.GetTransform().GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.useGravity = false;
            rb.isKinematic = true;
            rb.constraints = RigidbodyConstraints.FreezeAll;
        }

        return item;
    }

    //==================================개별 기능======================================

    // 상호작용1: J / Button South
    public abstract void Interact(Player player);
    // 상호작용2: K / Button West
    public abstract void InteractSecondary(Player player);

    //=================================데이터 전달======================================

    public Transform GetTransform()
    {
        return transform;
    }
}
