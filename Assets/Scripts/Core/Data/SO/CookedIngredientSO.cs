using System;
using UnityEngine;

[CreateAssetMenu(fileName = "CookedIngredientSO", menuName = "SO/DataSO/CookedIngredientSO")]
public class CookedIngredientSO : ScriptableObject
{
    public int id;
    public string cookedIngredientName;
    public string nameEng;
    public int?[] ingredientIds;

    public CookwareType cookwareType;
    public GameObject model;
}
