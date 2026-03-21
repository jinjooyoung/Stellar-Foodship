using UnityEngine;
using UnityEngine.InputSystem.XR;

public abstract class Pickable : MonoBehaviour, IInteractable
{
    private Player player;

    void Awake()
    {
        player = GetComponent<Player>();
    }

    public Transform GetTransform()
    {
        return transform;
    }

    // 상호작용1: "집기 / 놓기" 공통 처리
    public virtual void Interact(Player player)
    {
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
