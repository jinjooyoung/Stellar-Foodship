using System;
using UnityEngine;

[CreateAssetMenu(fileName = "DishSO", menuName = "SO/DataSO/DishSO")]
public class DishSO : ScriptableObject
{
    public int id;
    public string dishName;
    public string nameEng;
    public int?[] ingredientIds;
    public int score;

    public CookwareType cookwareType;
    public Sprite icon;
    public GameObject model;
}
