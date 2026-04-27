using UnityEngine;

public class TrashCan : NonPickable
{
    [Header("Trash Can Settings")]
    
    private bool _canPlace = false;
    public override bool canPlace => _canPlace;

    public override void Interact(Player player)
    {
        
        if (player.heldItem == null)
        {
            Debug.Log("손에 든 아이템이 없습니다.");
            return;
        }

       
        if (player.heldItem is Ingredient ingredient)
        {
            Debug.Log("재료를 쓰레기통에 버립니다.");

            
            GameObject temp = ingredient.gameObject;

            
            player.heldItem = null;

            
            Destroy(temp);
        }
        
        else if (player.heldItem is Cookware cookware)
        {
            Debug.Log("조리도구를 비웁니다.");

            cookware.ClearIds();
        }
        
        else if (player.heldItem is Dish dish)
        {
            Debug.Log("접시를 비웁니다.");

            dish.ClearDishContents();
        }
    }

    public override void InteractSecondary(Player player)
    {
       
    }
}