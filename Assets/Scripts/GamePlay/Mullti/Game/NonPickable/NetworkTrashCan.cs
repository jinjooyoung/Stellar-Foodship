using Fusion;
using UnityEngine;

public class NetworkTrashCan : NewNonPickable
{
    public override bool CanPlace => false;

    //------------------------------------------------

    public override void Interact(NewPlayer player)
    {
        if (!Object.HasStateAuthority)
            return;

        if (player.HeldItem == null)
        {
            Debug.Log("손에 든 아이템이 없습니다.");
            return;
        }

        NewPickable pickable =
            player.HeldItem.GetComponent<NewPickable>();

        if (pickable == null)
            return;

        //------------------------------------------------
        // 재료 버리기
        //------------------------------------------------

        if (pickable is NetworkIngredient ingredient)
        {
            Debug.Log("재료를 쓰레기통에 버립니다.");

            player.SetHeldItem(null);

            Runner.Despawn(ingredient.Object);

            return;
        }

        //------------------------------------------------
        // 조리도구 비우기
        //------------------------------------------------

        if (pickable is NetworkCookware cookware)
        {
            Debug.Log("조리도구를 비웁니다.");

            cookware.Clear();

            return;
        }

        //------------------------------------------------
        // 접시 비우기
        //------------------------------------------------

        if (pickable is NetworkDish dish)
        {
            Debug.Log("접시를 비웁니다.");

            dish.ClearDish();

            return;
        }
    }

    //------------------------------------------------

    public override void InteractSecondary(NewPlayer player)
    {
    }
}