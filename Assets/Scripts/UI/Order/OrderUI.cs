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
    public IngredientSlotUI[] slots;

    public Order order;

    public void Init(int dishId, Order order, OrderManager manager)
    {
        this.order = order;

        DishSO data = DataManager.instance.dishDatabase.GetDishById(dishId);
        dishIcon.sprite = data.icon;

        this.manager = manager;
        timer.OnCompleted += HandleTimerComplete;

        for (int i = 0; i < 4; i++)
        {
            if (data.ingredientIds[i] == -1)
            {
                slots[i].gameObject.SetActive(false);

                continue;
            }

            slots[i].Init(data.ingredientIds[i]);
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
}
