using System;
using UnityEngine;

[CreateAssetMenu(fileName = "IngredientSO", menuName = "DataSO/IngredientSO")]
public class IngredientSO : ScriptableObject
{
    public int id;
    public string ingredientName;
    public string nameEng;
    public bool isCutable;

    public Sprite icon;
    public GameObject model;
}
