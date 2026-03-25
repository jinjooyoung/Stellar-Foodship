using System;
using UnityEngine;

[CreateAssetMenu(fileName = "AchievementSO", menuName = "SO/DataSO/AchievementSO")]
public class AchievementSO : ScriptableObject
{
    public int id;
    public string nameKr;
    public string description;
    public int goal;

    public AchievementType achievementType;
    public Sprite icon;
}
