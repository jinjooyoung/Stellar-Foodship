using UnityEngine;

public interface IInteractable
{
    void Interact(Player player);
    void InteractSecondary(Player player);
    Transform GetTransform();
}