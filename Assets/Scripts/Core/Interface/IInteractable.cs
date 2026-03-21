using UnityEngine;

public interface IInteractable
{
    void Interact(Player player);
    Transform GetTransform();
}