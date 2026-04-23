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
    public Image[] slots;

    public Order order;

    public void Init(int dishId, Order order, OrderManager manager)
    {
        this.order = order;

        DishSO data = DataManager.instance.dishDatabase.GetDishById(dishId);
        dishIcon.sprite = data.icon;

        int ingreCount = 0;

        this.manager = manager;
        timer.OnCompleted += HandleTimerComplete;

        // 재료 id의 수만큼 재료 ui 이미지를 켬
        foreach (int id in data.ingredientIds)
        {
            if (id == -1) continue;

            if (ingreCount >= slots.Length) break;

            ingreCount += GetCountIngre(id);
        }

        for(int i = 0; i < ingreCount; i++)
        {
            slots[i].gameObject.SetActive(true);
        }

        // 재료 id의 수만큼 재료 ui 이미지를 켬 나머지는 끔
        for (int i = ingreCount; i < slots.Length; i++)
        {
            slots[i].gameObject.SetActive(false);
        }

        SetImage();
    }

    public void SetImage()
    {
        int slotIndex = 0;

        foreach(int id in order.dish.ingredientIds)
        {
            if(id == -1) continue;

            if(id < 100)
            {
                slots[slotIndex].sprite = DataManager.instance.ingredientDatabase.GetIngredientById(id).icon;
                slotIndex++;
            }
            else
            {
                CookedIngredientSO cooked = DataManager.instance.cookedIngredientDatabase.GetCookedIngredientById(id);

                Sprite temp;

                for (int i = 0; i < 4; i++)
                {
                    int ingreId = cooked.ingredientIds[i];

                    if (ingreId == -1) continue;
                    
                    temp = DataManager.instance.ingredientDatabase.GetIngredientById(ingreId).icon;

                    slots[slotIndex].sprite = temp;
                    slotIndex++;
                }
            }
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

    // 아이디 받으면 그 id의 재료 갯수 리턴
    public int GetCountIngre(int id)
    {
        int count = 0;

        if(id > 0 && id < 100)
        {
            return 1;
        }
        else if (id < 200)
        {
            CookedIngredientSO cooked = DataManager.instance.cookedIngredientDatabase.GetCookedIngredientById(id);

            return FilterListCount(cooked);
        }

        return 0;
    }

    // 1차 조리품 데이터를 받으면 필요한 재료의 갯수 리턴
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
