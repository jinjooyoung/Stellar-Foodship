using System.Collections.Generic;
using Fusion;
using UnityEngine;

public class NetworkDish : NewPickable
{
    public override int ID => ResultId;

    [Networked]
    public int ResultId { get; set; }

    [Networked, Capacity(4), OnChangedRender(nameof(OnIngredientChanged))]
    NetworkArray<int> IngredientIds => default;

    [Networked]
    public int IngredientCount { get; set; }

    public CookingIconUI cookingIconUI;

    //------------------------------------------------

    public override void Spawned()
    {
        base.Spawned();

        OnIngredientChanged();
    }

    //------------------------------------------------

    void OnIngredientChanged()
    {
        cookingIconUI?.UpdateUI(GetIngredientList());
    }

    //------------------------------------------------

    public List<int> GetIngredientList()
    {
        List<int> list = new();

        for (int i = 0; i < IngredientCount; i++)
            list.Add(IngredientIds[i]);

        return list;
    }

    //------------------------------------------------

    public bool TryAddIngredient(NetworkIngredient ingredient)
    {
        if (!Object.HasStateAuthority)
            return false;

        if (IngredientCount >= 4)
            return false;

        bool canAdd =
            ingredient.ingredientData.isRawPlatable &&
            (!ingredient.ingredientData.isCutable ||
             ingredient.IsCut);

        if (!canAdd)
            return false;

        IngredientIds.Set(IngredientCount, ingredient.ID);

        IngredientCount++;

        Runner.Despawn(ingredient.Object);

        OnIngredientChanged();

        return true;
    }

    //------------------------------------------------

    public bool TryServe(NetworkCookware cookware)
    {
        if (!Object.HasStateAuthority)
            return false;

        if (!cookware.IsComplete)
            return false;

        if (cookware.IsBurnt)
            return false;

        if (IngredientCount >= 4)
            return false;

        if (cookware.IngredientCount == 0)
            return false;

        IngredientIds.Set(
            IngredientCount,
            cookware.ResultId);

        IngredientCount++;

        cookware.Clear();

        OnIngredientChanged();

        return true;
    }

    //------------------------------------------------

    public void ClearDish()
    {
        ResultId = -1;

        for (int i = 0; i < 4; i++)
            IngredientIds.Set(i, 0);

        IngredientCount = 0;

        OnIngredientChanged();
    }
}