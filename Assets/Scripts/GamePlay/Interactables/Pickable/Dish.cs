using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Dish : Pickable
{
    [Header("Dish Settings")]
    public int resultId;                                // 완성품 id
    public List<int> currentIngredientIds = new List<int>();
    public CookingIconUI cookingIconUI;

    public override int ID => resultId;

    private void Start()
    {
        cookingIconUI.UpdateUI(currentIngredientIds);
    }

    /*public override void Interact(Player player)
    {
        if (player.heldItem == null)
        {
            if (TryPickUp(player))
            {
                player.heldItem = this;
            }
        }
    }*/

    public override void InteractSecondary(Player player) { }

    /*private void AddIngredientDish(Player player, Pickable heldItem)
    {
        if (currentIngredientIds.Count >= 4) return;

        currentIngredientIds.Add(heldItem.ID);
        cookingIconUI?.UpdateUI(currentIngredientIds);
        DestroyIngredient(player, heldItem);
    }

    private void DestroyIngredient(Player player, Pickable pickable)
    {
        if (pickable is Ingredient)
        {
            Destroy(pickable.gameObject);
            player.heldItem = null;
        }
        else if (pickable is Cookware cookware)
        {
            cookware.ClearIds();
        }
    }*/

  
    public void ClearDishContents()
    {
        // 데이터 초기화
        resultId = -1;
        currentIngredientIds.Clear();

        // UI 동기화
        if (cookingIconUI != null)
        {
            cookingIconUI.UpdateUI(currentIngredientIds);
        }

        Debug.Log("접시의 데이터가 초기화되었습니다.");
    }
}