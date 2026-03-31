using UnityEngine;
using UnityEngine.InputSystem.XR;

public abstract class Pickable : MonoBehaviour, IInteractable
{
    // 재료든 조리도구(도구오브젝트 자체에 ID가 있는건 아니지만 도구에 들어있는 조리된 1차 조합물에 ID가 있으니)든 요리든 ID 있어서 선언함
    public abstract int ID { get; }

    // 생성될 때 플레이어와 충돌 판정 안 하도록 세팅
    void Awake()
    {
        Collider myCol = GetComponent<Collider>();
        if (myCol == null) return;

        // "Player" 태그가 붙은 오브젝트만 검색
        GameObject[] playerObjects = GameObject.FindGameObjectsWithTag("Player");
        foreach (var pObj in playerObjects)
        {
            Collider pCol = pObj.GetComponent<Collider>();
            if (pCol != null)
            {
                Physics.IgnoreCollision(myCol, pCol);
            }
        }
    }

    //==================================공통 기능======================================

    // 상호작용1: "집기 / 놓기" 공통 처리 | J / Button South
    public virtual void Interact(Player player)
    {
        Debug.Log($"{this.name} Pickable 상호작용 호출됨");

        if (player.heldItem != null) return;

        if (TryPickUp(player))
        {
            player.heldItem = this;
        }
    }
        /*들고 있을 때는 Pickable.Interact 호출 안 됨. 
    }     (Player.InteractPrimary에서 처리)*/

        // 상호작용2: 던지기 | K / Button West
    public virtual void InteractSecondary(Player player)
    {
        // 픽커블은 상호작용2키 필요 없을 듯 근데 혹시 모르니 일단 냅두고 나중에 확실해지면 인터페이스부터 코드 수정
    }

    // 픽커블 -> Player가 들기
    public virtual bool TryPickUp(Player player)
    {
        // NonPickable에서 떨어뜨리기
        NonPickable parentSlot = GetComponentInParent<NonPickable>();
        if (parentSlot != null)
        {
            parentSlot.TakeItem(player);
        }

        // 위치 이동
        Transform t = transform;
        t.SetParent(player.holdPoint);
        t.localPosition = Vector3.zero;
        t.localRotation = Quaternion.identity;

        // 물리 처리
        Rigidbody rb = GetComponent<Rigidbody>();
        Collider col = GetComponent<Collider>();

        if (rb != null)
        {
            rb.useGravity = false;
            rb.isKinematic = true;
            rb.constraints = RigidbodyConstraints.FreezeAll;
        }

        if (col != null)
        {
            col.enabled = false;
        }

        return true;
    }

    //=================================데이터 전달======================================

    public Transform GetTransform()
    {
        return transform;
    }
}
