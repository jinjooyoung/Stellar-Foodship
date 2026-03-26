using UnityEngine;

public class Ingredient : Pickable
{
    public int ingredientID;
    public bool isCut = false;

    public override int ID => ingredientID;
}
