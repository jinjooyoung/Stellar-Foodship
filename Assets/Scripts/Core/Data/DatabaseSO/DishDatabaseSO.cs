using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DishDatabaseSO", menuName = "SO/DatabaseSO/DishDataBaseSO")]
public class DishDatabaseSO : ScriptableObject
{
    public List<DishSO> dishes = new List<DishSO>();

    // 캐싱을 위한 딕셔너리
    private Dictionary<int, DishSO> dishById;     // ID로 요리SO 찾기

    public void Initialize()
    {
        dishById = new Dictionary<int, DishSO>();

        foreach (var dish in dishes)
        {
            dishById[dish.id] = dish;
        }
    }

    // ID로 요리SO 찾기
    public DishSO GetDishById(int id)
    {
        if (dishById == null)
        {
            Initialize();
        }

        if (dishById.TryGetValue(id, out DishSO dish))
            return dish;

        return null;
    }
}
