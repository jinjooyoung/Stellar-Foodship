using System;
using UnityEngine;

public class AchievementData
{
    public int id;
    public string nameKr;
    public string description;
    public int goal;
    public string achievementTypeString;

    [NonSerialized]
    public AchievementType achievementType;
    public string iconPath;

    // 문자열을 열거형으로 변환하는 메서드
    public void InitalizeEnums()
    {
        if (Enum.TryParse(achievementTypeString, out AchievementType parsedType))
        {
            achievementType = parsedType;
        }
        else
        {
            Debug.LogError($"아이템 '{achievementType}'에 유효하지 않은 아이템 타입 : {achievementTypeString}");
        }
    }
}
