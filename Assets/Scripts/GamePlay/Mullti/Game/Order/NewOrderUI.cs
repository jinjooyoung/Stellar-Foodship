using System;
using UnityEngine;
using UnityEngine.UI;

public class NewOrderUI : MonoBehaviour
{
    [Header("ÂüÁ¶")]
    public NetworkTimer timer;
    public NetworkOrderManager manager;

    [Header("UI")]
    public Image dishIcon;
    public Image[] slots;

    private int dishId;

    //------------------------------------------------

    public void Init(int dishId, NetworkOrderManager manager)
    {
        this.dishId = dishId;
        this.manager = manager;

        DishSO data = DataManager.instance.dishDatabase.GetDishById(dishId);

        if (data == null) return;

        dishIcon.sprite = data.icon;

        timer.OnCompleted += HandleTimerComplete;

        int ingreCount = 0;

        foreach (int id in data.ingredientIds)
        {
            if (id == -1)
                continue;

            if (ingreCount >= slots.Length)
                break;

            ingreCount += GetCountIngre(id);
        }

        for (int i = 0; i < ingreCount; i++)
        {
            slots[i].gameObject.SetActive(true);
        }

        for (int i = ingreCount; i < slots.Length; i++)
        {
            slots[i].gameObject.SetActive(false);
        }

        SetImage(data);
    }

    //------------------------------------------------

    void SetImage(DishSO dish)
    {
        int slotIndex = 0;

        foreach (int id in dish.ingredientIds)
        {
            if (id == -1)
                continue;

            if (id < 100)
            {
                slots[slotIndex].sprite = DataManager.instance.ingredientDatabase.GetIngredientById(id).icon;

                slotIndex++;
            }
            else
            {
                CookedIngredientSO cooked = DataManager.instance.cookedIngredientDatabase.GetCookedIngredientById(id);

                if (cooked == null)
                    continue;

                for (int i = 0; i < cooked.ingredientIds.Count; i++)
                {
                    int ingreId = cooked.ingredientIds[i];

                    if (ingreId == -1)
                        continue;

                    slots[slotIndex].sprite = DataManager.instance.ingredientDatabase.GetIngredientById(ingreId).icon;

                    slotIndex++;
                }
            }
        }
    }

    //------------------------------------------------

    void HandleTimerComplete()
    {
        manager.FailOrder(dishId);
    }

    //------------------------------------------------

    void OnDestroy()
    {
        timer.OnCompleted -= HandleTimerComplete;
    }

    //------------------------------------------------

    public int GetCountIngre(int id)
    {
        if (id >= 0 && id < 100)
        {
            return 1;
        }

        if (id < 200)
        {
            CookedIngredientSO cooked =
                DataManager.instance
                .cookedIngredientDatabase
                .GetCookedIngredientById(id);

            return FilterListCount(cooked);
        }

        return 0;
    }

    //------------------------------------------------

    int FilterListCount(
        CookedIngredientSO data)
    {
        if (data == null)
            return 0;

        int count = 0;

        foreach (int id in data.ingredientIds)
        {
            if (id == -1)
                continue;

            count++;
        }

        return count;
    }
}