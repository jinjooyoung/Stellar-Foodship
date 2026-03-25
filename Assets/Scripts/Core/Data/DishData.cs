using System;
using UnityEngine;

public class DishData
{
    public int id;
    public string dishName;
    public string nameEng;
    public int[] ingredientIds;
    public int score;
    public string cookwareTypeString;

    [NonSerialized]
    public CookwareType cookwareType;
    public string iconPath;
    public string modelPath;

    // 문자열을 열거형으로 변환하는 메서드
    public void InitalizeEnums()
    {
        if (Enum.TryParse(cookwareTypeString, out CookwareType parsedType))
        {
            cookwareType = parsedType;
        }
        else
        {
            Debug.LogError($"아이템 '{cookwareType}'에 유효하지 않은 아이템 타입 : {cookwareTypeString}");
        }
    }
}
