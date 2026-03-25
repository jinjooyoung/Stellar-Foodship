using System;
using UnityEngine;

[Serializable]
public class CookedIngredientData
{
    public int id;
    public string cookedIngredientName;
    public string nameEng;
    public int[] ingredientIds;
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
