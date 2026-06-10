using Fusion;
using UnityEngine;

[RequireComponent(typeof(NetworkObject))]
public abstract class NewNonPickable : NetworkBehaviour, INewInteractable
{
    [SerializeField] public Transform holdPoint;

    [Networked]
    public NewPickable HeldItem { get; set; }

    public virtual bool CanPlace => true;

    //================================================

    public virtual void Interact(NewPlayer player)
    {
        if (!Object.HasStateAuthority)
            return;

        // 손 비었음 -> 가져가기
        if (player.HeldItem == null)
        {
            TakeItem(player);
            return;
        }

        // 손에 있음 -> 올리기
        NewPickable item =
            player.HeldItem.GetComponent<NewPickable>();

        if (item == null)
            return;

        if (TryPlaceItem(item))
        {
            player.SetHeldItem(null);
        }
    }

    public virtual void InteractSecondary(NewPlayer player)
    {

    }

    //================================================
    // 아이템 올리기
    //================================================

    protected virtual bool TryPlaceItem(NewPickable item)
    {
        if (!Object.HasStateAuthority)
            return false;

        if (!CanPlace)
            return false;

        if (item == null)
            return false;

        // 이미 뭔가 있으면 합성 시도
        if (HeldItem != null)
        {
            NewPickable result = TryCombine(item);

            if (result == null)
                return false;

            HeldItem = result;

            result.transform.position =
                holdPoint.position;

            result.transform.rotation =
                holdPoint.rotation;

            OnItemPlaced(result);

            return true;
        }

        // 그냥 올리기
        HeldItem = item;

        item.Place(this);

        item.transform.position =
            holdPoint.position;

        item.transform.rotation =
            holdPoint.rotation;

        OnItemPlaced(item);

        return true;
    }

    //================================================
    // 아이템 가져가기
    //================================================

    protected virtual bool TakeItem(NewPlayer player)
    {
        if (!Object.HasStateAuthority)
            return false;

        if (HeldItem == null)
            return false;

        player.SetHeldItem(HeldItem);

        NewPickable item =
            HeldItem.GetComponent<NewPickable>();

        if (item == null)
            return false;

        item.PickUp(player.Object.InputAuthority);

        HeldItem = null;

        OnItemTaken(item);

        return true;
    }

    //================================================
    // 조합
    //================================================

    protected virtual NewPickable TryCombine(NewPickable incoming)
    {
        if (HeldItem == null)
            return null;

        NewPickable placed =
            HeldItem.GetComponent<NewPickable>();

        if (placed == null)
            return null;

        // Ingredient -> Cookware

        if (placed is NetworkIngredient ing1 &&
            incoming is NetworkCookware cook1)
        {
            if (cook1.TryAddIngredient(ing1))
                return cook1;
        }

        if (placed is NetworkCookware cook2 &&
            incoming is NetworkIngredient ing2)
        {
            if (cook2.TryAddIngredient(ing2))
                return cook2;
        }

        // Ingredient -> Dish

        if (placed is NetworkIngredient ing3 &&
            incoming is NetworkDish dish1)
        {
            if (dish1.TryAddIngredient(ing3))
                return dish1;
        }

        if (placed is NetworkDish dish2 &&
            incoming is NetworkIngredient ing4)
        {
            if (dish2.TryAddIngredient(ing4))
                return dish2;
        }

        // Cookware -> Dish

        if (placed is NetworkCookware cook3 &&
            incoming is NetworkDish dish3)
        {
            if (dish3.TryServe(cook3))
                return dish3;
        }

        if (placed is NetworkDish dish4 &&
            incoming is NetworkCookware cook4)
        {
            if (dish4.TryServe(cook4))
                return dish4;
        }

        return null;
    }

    //================================================

    protected virtual void OnItemPlaced(NewPickable item)
    {

    }

    protected virtual void OnItemTaken(NewPickable item)
    {

    }

    //================================================

    public Transform GetTransform()
    {
        return transform;
    }
}