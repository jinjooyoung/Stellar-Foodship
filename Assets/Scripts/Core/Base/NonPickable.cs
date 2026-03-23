using UnityEngine;

public abstract class NonPickable : MonoBehaviour, IInteractable
{
    [Header("½½·Ô")]
    public IInteractable heldItem;
    public Transform holdPoint;

    protected NonPickable()
    {
    }

    public Transform GetTransform()
    {
        return transform;
    }

    public abstract void Interact(Player player);
    public abstract bool TryPlaceItem(Pickable item);

    public virtual IInteractable TakeItem()
    {
        if (heldItem == null) return null;

        IInteractable item = heldItem;
        heldItem = null;

        item.GetTransform().SetParent(null);

        return item;
    }
}
