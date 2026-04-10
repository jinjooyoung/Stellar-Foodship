using System.Collections.Generic;
using UnityEngine;

public static class CookingSystem
{
    //재료 id가 0~199까지
    private const int TOTAL_ITEM_COUNT = 200;

    //인풋과 레시피 배열이 일치하는 bool 반환하는 함수
    private static bool IsMatch(List<int> input, List<int> recipe, CookwareType inputType, CookwareType recipeType)
    {
        // 인풋이랑 레시피 재료 수 다르면 false
        if (GetRecipeCount(input) != GetRecipeCount(recipe)) return false;

        // 인풋이랑 레시피 타입 다르면 false
        if (inputType != recipeType) return false;

        // 크기 TOTAL_ITEM_COUNT인 count 배열 생성
        int[] count = new int[TOTAL_ITEM_COUNT];

        // 인풋으로 받은 재료 id를 인덱스++
        for (int i = 0; i < input.Count; i++)
        {
            if (input[i] == -1) continue;

            count[input[i]]++;
        }

        // 레시피 재료 id에 해당하는 인덱스 --
        for (int i = 0; i < recipe.Count; i++)  
        {
            if (recipe[i] == -1) continue;

            count[recipe[i]]--;  
        }

        // count 배열 전체 반복 -> 0 아닌거 하나라도 있으면 false
        for (int i = 0; i < TOTAL_ITEM_COUNT; i++)
        {
            if (count[i] != 0) return false;
        }
        return true;
    }

    // 1차 조리품 ID 반환 (없으면 100 반환)
    public static int GetCookedIngredientId(List<int> inputIds, CookwareType inputType, bool isBurnt)
    {
        // 탄 음식이면 바로 100 반환
        if (isBurnt) return 100;
        CookedIngredientDatabaseSO db = DataManager.instance.cookedIngredientDatabase;

        foreach (var SO in db.cookedIngredients)
        {
            if (IsMatch(inputIds, SO.ingredientIds, inputType, SO.cookwareType))
            {
                return SO.id;
            }
        }

        return 100; // 실패 조리품 id
    }

    // 요리 ID 반환 (없으면 200 반환)
    public static int GetDishId(List<int> inputIds, CookwareType inputType)
    {
        DishDatabaseSO db = DataManager.instance.dishDatabase;

        foreach (var SO in db.dishes)
        {
            if (IsMatch(inputIds, SO.ingredientIds, inputType, SO.cookwareType))
            {
                return SO.id;
            }
        }

        return 200; // 실패 요리 id
    }

    // 레시피 배열에서 재료 수 리턴하는 함수
    private static int GetRecipeCount(List<int> list)
    {
        int count = 0;

        for(int i = 0; i < list.Count; i++)
        {
            if (list[i] >= 0) count++;
        }

        return count;
    }
}