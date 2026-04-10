using System;
using UnityEngine;
using UnityEngine.UI;

public class OrderUI : MonoBehaviour
{
    [Header("참조")]
    public Timer timer;
    public OrderManager manager;

    [Header("UI 오브젝트")]
    public Image dishIcon;
    public Image[] slots;    // 1차 조리품 아이콘

    public Order order;

    public void Init(int dishId, Order order, OrderManager manager)
    {
        this.order = order;

        DishSO data = DataManager.instance.dishDatabase.GetDishById(dishId);
        dishIcon.sprite = data.icon;

        int slotIndex = 0;

        this.manager = manager;
        timer.OnCompleted += HandleTimerComplete;

        // 재료 id의 수만큼 재료 ui 이미지를 켬
        foreach (int id in data.ingredientIds)
        {
            if (id == -1) continue;

            if (slotIndex >= slots.Length) break;

            slots[slotIndex].gameObject.SetActive(true);
            InitIngreImage(id, slotIndex);

            slotIndex++;
        }

        // 재료 id의 수만큼 재료 ui 이미지를 켬 나머지는 끔
        for (int i = slotIndex; i < slots.Length; i++)
        {
            slots[i].gameObject.SetActive(false);
        }
    }

    void HandleTimerComplete()
    {
        manager.FailOrder(order);
    }

    void OnDestroy()
    {
        timer.OnCompleted -= HandleTimerComplete;
    }

    public void InitIngreImage(int id, int index)
    {
        if (id < 100)    // 재료 아이콘
        {
            slots[index].sprite = DataManager.instance.ingredientDatabase.GetIngredientById(id).icon;
        }
        else    // 1차 조리품 아이콘 적용
        {
            slots[index].sprite = DataManager.instance.cookedIngredientDatabase.GetCookedIngredientById(id).icon;
        }
    }
}
