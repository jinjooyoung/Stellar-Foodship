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

  

    public override void Interact(Player player)
    {
        Debug.Log("Dish Interact 호출됨");
        Debug.Log($"player.heldItem 타입 : {player.heldItem?.GetType().Name}");

        if (player.heldItem != null)
        {
            // 재료인지 확인
            if (player.heldItem is Ingredient ingredient)
            {
                Debug.Log("재료 케이스 진입");
                Debug.Log($"ingredientData: {ingredient.ingredientData}");
                Debug.Log($"isRawPlatable: {ingredient.ingredientData.isRawPlatable}");  
                Debug.Log($"isCutable: {ingredient.ingredientData.isCutable}");          
                Debug.Log($"isCut: {ingredient.isCut}");

                
                if (!ingredient.ingredientData.isRawPlatable)
                {
                    Debug.Log("생으로 담을 수 없는 재료!");
                    return;
                }

                
                if (ingredient.ingredientData.isCutable)
                {
                    
                    if (!ingredient.isCut)
                    {
                        Debug.Log("재료가 썰려있지 않아, 담을 수 없습니다.");
                        return;
                    }
                }

                
                AddIngredientDish(player, ingredient);
            }
            
            else if (player.heldItem is Cookware cookware)
            {
                Debug.Log("조리도구 케이스 진입");

                
                if (cookware.currentIngredientIds.Count == 0)
                {
                    Debug.Log("조리도구가 비어있습니다.");
                    return;
                }

                
                if (!cookware.isComplete)
                {
                    Debug.Log("조리가 완료되지 않았습니다.");
                    return;
                }

                
                int resultId = CookingSystem.GetCookedIngredientId(
                    cookware.currentIngredientIds,
                    cookware.cookwareType,
                    cookware.isBurnt
                );

                cookware.resultId = resultId;

               
                AddIngredientDish(player, cookware);
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

    public override void InteractSecondary(Player player)
    {
    }

    

    private void AddIngredientDish(Player player, Pickable heldItem)
    {
        Debug.Log("AddIngredientDish 호출됨");

        
        if (currentIngredientIds.Count >= 4)
        {
            Debug.Log("접시가 이미 가득 차서 넣을 수 없습니다!");
            return;
        }

        Debug.Log("빈 자리 있음");

        
        currentIngredientIds.Add(heldItem.ID);

        
        Debug.Log("UI 업데이트");
        cookingIconUI?.UpdateUI(currentIngredientIds);

        // 재료/조리도구 제거
        Debug.Log("DestroyIngredient 호출");
        DestroyIngredient(player, heldItem);
    }

    

    private void DestroyIngredient(Player player, Pickable pickable)
    {
        if (pickable is Ingredient)
        {
            // 재료면 오브젝트 파괴
            Destroy(pickable.gameObject);
            player.heldItem = null;
        }
        else if (pickable is Cookware cookware)
        {
            // 조리도구면 배열 초기화
            cookware.ClearIds();
        }
    }
}