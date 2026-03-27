using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "IngredientDatabaseSO", menuName = "SO/DatabaseSO/IngredientDataBaseSO")]
public class IngredientDatabaseSO : ScriptableObject
{
    public List<IngredientSO> ingredients = new List<IngredientSO>();

    // 캐싱을 위한 딕셔너리
    private Dictionary<int, IngredientSO> ingredientById;     // ID로 재료SO 찾기

    public void Initialize()
    {
        ingredientById = new Dictionary<int, IngredientSO>();

        foreach (var ingredient in ingredients)
        {
            ingredientById[ingredient.id] = ingredient;
        }
    }

    // ID로 재료SO 찾기
    public IngredientSO GetIngredientById(int id)
    {
        if (ingredientById == null)
        {
            Initialize();
        }

        if (ingredientById.TryGetValue(id, out IngredientSO ingredient))
            return ingredient;

        return null;
    }
}
