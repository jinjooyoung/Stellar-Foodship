using Fusion;
using UnityEngine;

[RequireComponent(typeof(NetworkObject))]
public abstract class NewNonPickable : NetworkBehaviour, INewInteractable
{
    [SerializeField] protected Transform holdPoint;

    [Networked]
    public NetworkObject HeldItem { get; set; }

    public virtual bool CanPlace => true;

    //========================================

    public virtual void Interact(NewPlayer player)
    {
        // 플레이어가 들고 있음 -> 내려놓기
        if (player.HeldItem != null)
        {
            PlaceItem(player);
        }
        // 아무것도 안 들고 있음 -> 꺼내기
        else
        {
            TakeItem(player);
        }
    }

    public virtual void InteractSecondary(NewPlayer player)
    {

    }

    //========================================

    protected virtual bool PlaceItem(NewPlayer player)
    {
        if (!Object.HasStateAuthority)
            return false;

        if (!CanPlace)
            return false;

        if (HeldItem != null)
            return false;

        HeldItem = player.HeldItem;

        NewPickable item =
            HeldItem.GetComponent<NewPickable>();

        item.PickUp(default);

        item.transform.position = holdPoint.position;
        item.transform.rotation = holdPoint.rotation;

        player.SetHeldItem(null);

        OnItemPlaced(item);

        return true;
    }

    protected virtual bool TakeItem(NewPlayer player)
    {
        if (!Object.HasStateAuthority)
            return false;

        if (HeldItem == null)
            return false;

        player.SetHeldItem(HeldItem);

        NewPickable item =
            HeldItem.GetComponent<NewPickable>();

        item.PickUp(player.Object.InputAuthority);

        HeldItem = null;

        OnItemTaken(item);

        return true;
    }

    //========================================

    protected virtual void OnItemPlaced(NewPickable item)
    {

    }

    protected virtual void OnItemTaken(NewPickable item)
    {

    }

    //========================================

    public Transform GetTransform()
    {
        return transform;
    }
}