using UnityEngine;
using UnityEngine.UI;

public class IngredientSlotUI : MonoBehaviour
{
    [Header("UI 오브젝트")]
    public Image cookedIcon;
    public Image[] ingredientIcons;
    public Image cookTypeIcon;

    public void Init(int id)
    {
        if(id < 100)    // 재료면 다른거 다 끄고 재료 아이콘오브젝트만 켜고 스프라이트 적용하기
        {
            cookedIcon.gameObject.SetActive(false);
            cookTypeIcon.gameObject.SetActive(false);

            foreach(var image in ingredientIcons)
            {
                image.gameObject.SetActive(false);
            }

            ingredientIcons[0].gameObject.SetActive(true);

            ingredientIcons[0].sprite = DataManager.instance.ingredientDatabase.GetIngredientById(id).icon;
        }
        else    // 1차 조리품이면 조리품 아이콘 적용, 조리품에 필요한 재료 아이콘 적용, 해당 쿡웨어타입 아이콘까지 리소스 폴더에서 불러와서 적용
        {
            CookedIngredientSO data = DataManager.instance.cookedIngredientDatabase.GetCookedIngredientById(id);

            cookedIcon.sprite = data.icon;
            cookedIcon.gameObject.SetActive(true);

            SetIngredients(data.ingredientIds);

            cookTypeIcon.sprite = data.cookTypeIcon;
            cookTypeIcon.gameObject.SetActive(true);
        }
    }

    private void SetIngredients(int?[] ids)
    {
        IngredientDatabaseSO db = DataManager.instance.ingredientDatabase;

        for(int i = 0; i < 4; i++)
        {
            if (ids[i].HasValue)
            {
                ingredientIcons[i].sprite = db.GetIngredientById(ids[i].Value).icon;
                ingredientIcons[i].gameObject.SetActive(true);
            }
            else
            {
                ingredientIcons[i].gameObject.SetActive(false);
            }
        }
    }
}
