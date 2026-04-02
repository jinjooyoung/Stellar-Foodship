using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CookingIconUI : MonoBehaviour
{
    public Image[] slots; // 최대 16칸 (접시는 16개까지 될수도)

    public void UpdateUI(int?[] currentIds)
    {
        var ingredientDB = DataManager.instance.ingredientDatabase;
        var cookedDB = DataManager.instance.cookedIngredientDatabase;

        List<Sprite> sprites = new List<Sprite>();

        // 데이터 펼치기
        for (int i = 0; i < currentIds.Length; i++)
        {
            if (!currentIds[i].HasValue)
                continue;

            int id = currentIds[i].Value;

            // 재료
            if (id < 100)
            {
                var ingredient = ingredientDB.GetIngredientById(id);
                if (ingredient != null)
                    sprites.Add(ingredient.icon);
            }
            else // 1차 조리품
            {
                var cooked = cookedDB.GetCookedIngredientById(id);
                if (cooked == null)
                    continue;

                // 내부 레시피 펼치기
                foreach (int recipeId in cooked.ingredientIds)
                {
                    var ingredient = ingredientDB.GetIngredientById(recipeId);
                    if (ingredient != null)
                        sprites.Add(ingredient.icon);
                }
            }
        }

        // UI 반영
        int count = sprites.Count;

        for (int i = 0; i < slots.Length; i++)
        {
            if (i < count)
            {
                slots[i].sprite = sprites[i];
                slots[i].gameObject.SetActive(true);
            }
            else
            {
                slots[i].gameObject.SetActive(false);
            }
        }

        // 전체 숨김
        gameObject.SetActive(count > 0);
    }
}
