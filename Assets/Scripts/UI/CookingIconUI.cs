using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CookingIconUI : MonoBehaviour
{
    public Image[] slots; // 최대 4칸

    public void UpdateUI(int?[] currentIds)
    {
        if (DataManager.instance == null) return;

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
                if (cooked != null)
                    sprites.Add(cooked.icon);
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
