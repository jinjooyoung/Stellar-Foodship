using UnityEngine;
using UnityEngine.UI;

// UI 구조 바꿔서 이제는 필요없는 스크립트인데 혹시 모르니 일단 냅둠
public class IngredientSlotUI : MonoBehaviour
{
    [Header("UI 오브젝트")]
    public Image[] ingredientIcons;
    public Image underLine;
    public Image cookTypeIcon;
    public Image cookTypeBG;

    Color orange = new Color(255f / 255f, 138f / 255f, 44f / 255f);
    Color yellow = new Color(255f / 255f, 228f / 255f, 48f / 255f);
    Color green = new Color(124f / 255f, 222f / 255f, 90f / 255f);
    Color purple = new Color(175f / 255f, 155f / 255f, 231f / 255f);
    Color pink = new Color(247f / 255f, 154f / 255f, 213f / 255f);

    public void Init(int id)
    {
        if(id < 100)    // 재료면 다른거 다 끄고 재료 아이콘오브젝트만 켜고 스프라이트 적용하기
        {
            foreach(var image in ingredientIcons)
            {
                image.gameObject.SetActive(false);
            }
            underLine.gameObject.SetActive(false);
            cookTypeIcon.gameObject.SetActive(false);
            cookTypeBG.gameObject.SetActive(false);

            ingredientIcons[0].gameObject.SetActive(true);

            ingredientIcons[0].sprite = DataManager.instance.ingredientDatabase.GetIngredientById(id).icon;
        }
        else    // 1차 조리품이면 조리품에 필요한 재료 아이콘 적용, 해당 쿡웨어타입 아이콘까지 리소스 폴더에서 불러와서 적용
        {
            CookedIngredientSO data = DataManager.instance.cookedIngredientDatabase.GetCookedIngredientById(id);





            underLine.gameObject.SetActive(true);
            cookTypeIcon.gameObject.SetActive(true);
            cookTypeBG.gameObject.SetActive(true);

            cookTypeIcon.sprite = data.cookTypeIcon;

            Color temp;

            switch (data.cookwareType)
            {
                case CookwareType.Pan:
                    temp = orange;
                    break;
                case CookwareType.Pot:
                    temp = yellow;
                    break;
                case CookwareType.Steamer:
                    temp = green;
                    break;
                case CookwareType.MixerCup:
                    temp = purple;
                    break;
                case CookwareType.FryerBasket:
                    temp = pink;
                    break;
                default:
                    temp = Color.white;
                    break;
            }

            underLine.color = temp;
            cookTypeBG.color = temp;
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

    private int FilterListCount(CookedIngredientSO data)
    {
        int count = 0;

        for (int i = 0; i < data.ingredientIds.Count; i++)
        {
            if (data.ingredientIds[i] == -1) continue;

            count++;
        }

        return count;
    }
}
