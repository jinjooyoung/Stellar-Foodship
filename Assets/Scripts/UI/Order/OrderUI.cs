using System;
using UnityEngine;
using UnityEngine.UI;

public class OrderUI : MonoBehaviour
{
    [Header("타이머")]
    public Timer timer;

    [Header("UI 오브젝트")]
    public Image dishIcon;
    public IngredientSlotUI[] slots;    // 1차 조리품, 재료, 조리 방법이 다 포함 된 레시피 UI 구성품 클래스

    public Order order;



    public void Init(int dishId, Order order)
    {
        this.order = order;

        DishSO data = DataManager.instance.dishDatabase.GetDishById(dishId);
        dishIcon.sprite = data.icon;

        int slotIndex = 0;

        foreach(int? id in data.ingredientIds)
        {
            if (!id.HasValue) continue;

            if (slotIndex >= slots.Length) break;

            slots[slotIndex].gameObject.SetActive(true);
            slots[slotIndex].Init((int)id);

            slotIndex++;
        }

        for(int i = slotIndex; i < slots.Length; i++)
        {
            slots[i].gameObject.SetActive(false);
        }
    }

    internal void Init(object dishId, Order order)
    {
        throw new NotImplementedException();
    }
}
