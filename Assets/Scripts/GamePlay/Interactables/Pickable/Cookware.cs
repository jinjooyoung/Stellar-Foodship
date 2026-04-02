using System.Linq;
using UnityEngine;

public class Cookware : Pickable
{
 
    [Header("Cooking Settings")]
    public int resultId; 
    public int?[] currentIngredientIds = new int?[4]; 

   
    public override int ID => resultId;

  
    public override void Interact(Player player)
    {
       
        if (player.heldItem != null)
        {
            
            if (player.heldItem is Ingredient ingredient)
            {
                HandleIngredientInput(player, ingredient);
            }
        }
        else
        {
           
            if (TryPickUp(player))
            {
                player.heldItem = this;
            }
        }
    }

    private void HandleIngredientInput(Player player, Ingredient ingredient)
    {
        
        bool canAdd = !ingredient.ingredientData.isCutable || ingredient.isCut;

        if (canAdd)
        {
            AddIngredient(player, ingredient);
        }
        else
        {
            Debug.Log("재료가 썰리지 않아 넣을 수 없습니다.");
        }
    }

    private void AddIngredient(Player player, Ingredient ingredient)
    {
        
        if (!currentIngredientIds.Any(id => id == null))
        {
            Debug.Log("조리 도구가 이미 가득 찼습니다");
            return;
        }

        for (int i = 0; i < currentIngredientIds.Length; i++)
        {
            if (currentIngredientIds[i] == null)
            {
                currentIngredientIds[i] = ingredient.ingredientID;
                break;
            }
        }

        
        Destroy(ingredient.gameObject);
        player.heldItem = null;
    }


    public override void InteractSecondary(Player player)
    {
       
    }
}