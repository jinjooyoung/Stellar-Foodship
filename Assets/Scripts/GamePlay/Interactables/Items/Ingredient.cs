using UnityEngine;

public class Ingredient : Pickable
{
    public int ingredientID;
    public bool isCut = false;

    public override int ID => ingredientID;

    public void OnCutComplete()
    {
        isCut = true;
        Debug.Log("타이머 종료됨 재료 썰림 이벤트 호출됨");
    }
}
