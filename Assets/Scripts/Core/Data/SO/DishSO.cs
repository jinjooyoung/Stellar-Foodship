using System.Collections.Generic;
using System;
using UnityEngine;

[CreateAssetMenu(fileName = "DishSO", menuName = "SO/DataSO/DishSO")]
public class DishSO : ScriptableObject
{
    public int id;
    public string dishName;
    public string nameEng;
    public List<int> ingredientIds = new List<int>();
    public int score;

    public CookwareType cookwareType;
    public Sprite icon;
    public GameObject model;
    internal int dishId;
}
