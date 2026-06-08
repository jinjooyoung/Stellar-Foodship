using UnityEngine;

public interface INewInteractable
{
    void Interact(NewPlayer player);
    void InteractSecondary(NewPlayer player);
    Transform GetTransform();
}
