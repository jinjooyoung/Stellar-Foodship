using UnityEngine;
using UnityEngine.InputSystem.XR;

public abstract class Pickable : MonoBehaviour, IInteractable
{
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

    public Transform GetTransform()
    {
        return transform;
    }

    // 상호작용1: "집기 / 놓기" 공통 처리
    public virtual void Interact(Player player)
    {
        Debug.Log($"{this.name} Pickable 상호작용 호출됨");
        if (player.heldItem == null)
        {
            player.Pickup(this);
        }
        else
        {
            player.Drop();
        }
    }

    // 상호작용2: 자식마다 다르게
    public abstract void InteractSecondary(Player player);
}
