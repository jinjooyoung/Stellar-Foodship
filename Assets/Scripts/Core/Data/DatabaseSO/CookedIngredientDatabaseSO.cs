using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CookedIngredientDatabaseSO", menuName = "SO/DatabaseSO/CookedIngredientDataBaseSO")]
public class CookedIngredientDatabaseSO : ScriptableObject
{
    public List<CookedIngredientSO> cookedIngredients = new List<CookedIngredientSO>();

    // 캐싱을 위한 딕셔너리
    private Dictionary<int, CookedIngredientSO> cookedIngredientById;     // ID로 1차 조리품SO 찾기

    public void Initialize()
    {
        cookedIngredientById = new Dictionary<int, CookedIngredientSO>();

        foreach (var cookedIngredient in cookedIngredients)
        {
            cookedIngredientById[cookedIngredient.id] = cookedIngredient;
        }
    }

    // ID로 1차 조리품SO 찾기
    public CookedIngredientSO GetCookedIngredientById(int id)
    {
        if (cookedIngredientById == null)
        {
            Initialize();
        }

        if (cookedIngredientById.TryGetValue(id, out CookedIngredientSO cookedIngredient))
            return cookedIngredient;

        return null;
    }
}
