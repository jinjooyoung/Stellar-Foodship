using Fusion;
using UnityEngine;

public class NetworkDishSubmissionCounter : NewNonPickable
{
    [Header("Reference")]
    [SerializeField]
    private NetworkOrderManager orderManager;

    [SerializeField]
    private NetworkDishReturner dishReturner;

    public override bool CanPlace => false;

    //------------------------------------------------

    public override void Interact(NewPlayer player)
    {
        if (!Object.HasStateAuthority)
            return;

        if (player.HeldItem == null)
            return;

        NetworkDish dish = player.HeldItem.GetComponent<NetworkDish>();

        if (dish == null)
            return;

        int resultId = CookingSystem.GetDishId(dish.GetIngredientList(), CookwareType.Plate);

        bool success = orderManager.TrySubmitDish(resultId);

        if (success)
        {
            Debug.Log("주문 성공");
        }
        else
        {
            Debug.Log("주문 실패");
        }

        player.SetHeldItem(null);

        dishReturner.ReturnDish();

        Destroy(dish.cookingIconUI.gameObject);
        Runner.Despawn(dish.Object);
    }

    //------------------------------------------------

    public override void InteractSecondary(NewPlayer player)
    {
    }
}