using System;
using UnityEngine;

[Serializable]
public class IngredientData
{
    public int id;
    public string ingredientName;
    public string nameEng;
    public bool isCutable;

    [NonSerialized]
    public string iconPath;
    public string modelPath;
    public string cutModelPath;
    public string cookingModelPath;
    public string onDishModelPath;
}
