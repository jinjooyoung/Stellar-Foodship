using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CookedIngredientSO", menuName = "SO/DataSO/CookedIngredientSO")]
public class CookedIngredientSO : ScriptableObject
{
    public int id;
    public string cookedIngredientName;
    public string nameEng;
    public List<int> ingredientIds = new List<int>();

    public CookwareType cookwareType;
    public Sprite cookTypeIcon;
    public Sprite icon;
    public GameObject model;
}
