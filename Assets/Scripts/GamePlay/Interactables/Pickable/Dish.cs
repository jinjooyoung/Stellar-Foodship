using System.Linq;
using UnityEngine;

public class Dish : Pickable
{

    [Header("Dish Settings")]
    public int resultId;                                // 완성품 id
    [SerializeField] public int?[] currentIngredientIds = new int?[4];  // 갖고있는 재료, 1차 조리품 id 배열
    public CookingIconUI cookingIconUI;

    public override int ID => resultId;

    //====================================Interact====================================

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

                // 생으로 담을 수 없는 재료면 return
                if (!ingredient.ingredientData.isRawPlatable)
                {
                    Debug.Log("생으로 담을 수 없는 재료!");
                    return;
                }

                // 썰 수 있는 재료인지 확인
                if (ingredient.ingredientData.isCutable)
                {
                    // 썰려있지 않으면 담을 수 없음
                    if (!ingredient.isCut)
                    {
                        Debug.Log("재료가 썰려있지 않아, 담을 수 없습니다.");
                        return;
                    }
                }

                // 재료 추가 함수 실행
                AddIngredientDish(player, ingredient);
            }
            // 조리도구인지 확인
            else if (player.heldItem is Cookware cookware)
            {
                Debug.Log("조리도구 케이스 진입");

                // 조리도구가 비어있으면
                if (!cookware.HasAnyValue(cookware.currentIngredientIds))
                {
                    Debug.Log("조리도구가 비어있습니다.");
                    return;
                }

                // 조리 완료되지 않았으면
                if (!cookware.isComplete)
                {
                    Debug.Log("조리가 완료되지 않았습니다.");
                    return;
                }

                // 조리도구 조리 결과 id 계산 후 조리도구로 넘겨줌
                int resultId = CookingSystem.GetCookedIngredientId(
                    cookware.currentIngredientIds,
                    cookware.cookwareType,
                    cookware.isBurnt
                );

                cookware.resultId = resultId;

                // 재료 추가 함수 실행
                AddIngredientDish(player, cookware);
            }
        }
        else
        {
            // 안 들고 있으면 집기
            if (TryPickUp(player))
            {
                player.heldItem = this;
            }
        }
    }

    public override void InteractSecondary(Player player)
    {
    }

    //====================================재료 추가====================================

    private void AddIngredientDish(Player player, Pickable heldItem)
    {
        Debug.Log("AddIngredientDish 호출됨");

        // 접시가 이미 가득 찼으면
        if (!currentIngredientIds.Any(x => x == null))
        {
            Debug.Log("접시가 이미 가득 차서 넣을 수 없습니다!");
            return;
        }

        Debug.Log("빈 자리 있음");

        // 빈 자리에 재료 id 넣기
        for (int i = 0; i < currentIngredientIds.Length; i++)
        {
            if (currentIngredientIds[i] == null)
            {
                currentIngredientIds[i] = heldItem.ID;
                Debug.Log($"ID {heldItem.ID} 넣음");
                break;
            }
        }

        // UI 업데이트
        Debug.Log("UI 업데이트");
        cookingIconUI?.UpdateUI(currentIngredientIds);

        // 재료/조리도구 제거
        Debug.Log("DestroyIngredient 호출");
        DestroyIngredient(player, heldItem);
    }

    //====================================재료 제거====================================

    private void DestroyIngredient(Player player, Pickable pickable)
    {
        if (pickable is Ingredient)
        {
            // 재료면 오브젝트 파괴
            Destroy(pickable.gameObject);
        }
        else if (pickable is Cookware cookware)
        {
            // 조리도구면 배열 초기화
            cookware.ClearIds();
        }

        player.heldItem = null;
    }
}