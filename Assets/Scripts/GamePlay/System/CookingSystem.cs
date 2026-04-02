using System.Collections.Generic;
using UnityEngine;

public static class CookingSystem
{
    //재료 id가 0~199까지
    private const int TOTAL_ITEM_COUNT = 200;

    //인풋과 레시피 배열이 일치하는 bool 반환하는 함수
    private static bool IsMatch(int[] input, int[] recipe, CookwareType inputType, CookwareType recipeType)
    {
        // 인풋이랑 레시피 길이 다르면 false
        if (input.Length != recipe.Length) return false;

        // 인풋이랑 레시피 타입 다르면 false
        if (inputType != recipeType) return false;

        // 크기 TOTAL_ITEM_COUNT인 count 배열 생성
        int[] count = new int[TOTAL_ITEM_COUNT];

        // 인풋으로 받은 재료 id를 인덱스++
        for (int i = 0; i < input.Length; i++)
        {
            count[input[i]]++;
        }

        // 레시피 재료 id에 해당하는 인덱스 --
        for (int i = 0; i < recipe.Length; i++)  
        {
            count[recipe[i]]--;  
        }

        // count 배열 전체 반복 -> 0 아닌거 하나라고 있으면 false
        for (int i = 0; i < TOTAL_ITEM_COUNT; i++)
        {
            if (count[i] != 0) return false;
        }
        return true;
    }

    // null 제거하고 int[] 반환
    private static int[] FilterNull(int?[] arr)
    {
        List<int> list = new List<int>();

        foreach (int? id in arr)
        {
            if (id.HasValue)
            {
                list.Add(id.Value);
            }
        }
        return list.ToArray();
    }

    // 1차 조리품 ID 반환 (없으면 100 반환)
    public static int GetCookedIngredientId(int?[] inputIds, CookwareType inputType)
    {
        CookedIngredientDatabaseSO db = DataManager.instance.cookedIngredientDatabase;

        int[] input = FilterNull(inputIds);

        foreach (var data in db.cookedIngredients)
        {
            int[] recipe = FilterNull(data.ingredientIds);

            if (IsMatch(input, recipe, inputType, data.cookwareType))
            {
                return data.id;
            }
        }

        return 100; // 실패 조리품 id
    }

    // 요리 ID 반환 (없으면 200 반환)
    public static int GetDishId(int?[] inputIds, CookwareType inputType)
    {
        DishDatabaseSO db = DataManager.instance.dishDatabase;

        int[] input = FilterNull(inputIds);

        foreach (var data in db.dishes)
        {
            int[] recipe = FilterNull(data.ingredientIds);

            if (IsMatch(input, recipe, inputType, data.cookwareType))
            {
                return data.id;
            }
        }

        return 200; // 실패 요리 id
    }
}