using UnityEngine;
using UnityEngine.InputSystem.XR;
using static UnityEditor.Progress;
using static UnityEngine.GraphicsBuffer;

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

    //==================================공통 기능======================================

    // 상호작용1: "집기 / 놓기" 공통 처리 | J / Button South
    public virtual void Interact(Player player)
    {
        if (player.heldItem == null)
        {
            player.Pickup();
        }
        else
        {
            player.Drop();
        }
    }

    // 픽커블 -> Player가 들기
    public virtual bool TryPickUp(Player player)
    {
        Debug.Log("트라이픽업 호출");
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

    // 타이머 시작 재료, 조리 도구에는 타이머 O / 접시에는 타이머 X
    public virtual void StartProcess(float duration)
    {
        /*if (timer == null)
        {
            Debug.LogWarning($"{name} has no Timer!");
            return;
        }

        timer.StartTimer(duration);*/
    }

    //==================================개별 기능======================================

    // 상호작용2: 자식마다 다르게 | K / Button West
    public abstract void InteractSecondary(Player player);

    //=================================데이터 전달======================================

    public Transform GetTransform()
    {
        return transform;
    }
}
