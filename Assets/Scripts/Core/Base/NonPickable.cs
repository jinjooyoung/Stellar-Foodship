using UnityEngine;

public abstract class NonPickable : MonoBehaviour, IInteractable
{
    public Transform GetTransform()
    {
        return transform;
    }

    public abstract void Interact(Player player);
    public abstract bool TryPlaceItem(Pickable item);
}
