using UnityEngine;

public class DishSubmissionCounter : NonPickable
{
    [Header("참조")]
    [SerializeField] private OrderManager orderManager;
    public DishReturner dishReturner;

    [Header("이펙트")]
    public GameObject successEffectPrefab;
    public GameObject successEffect;

    public bool _canPlace;
    public override bool canPlace => _canPlace;

    public override void Interact(Player player)
    {
   
        if (player.heldItem == null)
        {
            return;
        }

        
        Dish dish = player.heldItem as Dish;

        
        if (dish == null)
        {
            return;
        }

      
        int resultId = CookingSystem.GetDishId(dish.currentIngredientIds, CookwareType.Plate);
      
        bool success = ClearSubmitDish(resultId);

        if (success)
        {
            successEffect = Instantiate(successEffectPrefab, transform.position + Vector3.up * 0.5f, Quaternion.identity);
            Debug.Log("주문 성공");
        }
        else
        {
            Debug.Log("주문 실패했습니다");
        }

        Destroy(dish.cookingIconUI.gameObject);
        Destroy(dish.gameObject);
        dishReturner.dishCount--;
        dishReturner.UpdateUI();
        player.heldItem = null;
    }

    public bool ClearSubmitDish(int id)
    {
        return orderManager.TrySubmitDish(id);
    }

   
    public override void InteractSecondary(Player player)
    {
       
    }
}