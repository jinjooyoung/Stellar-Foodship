using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AchievementDatabaseSO", menuName = "SO/DatabaseSO/AchievementDatabaseSO")]
public class AchievementDatabaseSO : ScriptableObject
{
    public List<AchievementSO> achievements = new List<AchievementSO>();

    // 캐싱을 위한 딕셔너리
    private Dictionary<int, AchievementSO> achievementById;     // ID로 업적SO 찾기

    public void Initialize()
    {
        achievementById = new Dictionary<int, AchievementSO>();

        foreach(var achievement in achievements)
        {
            achievementById[achievement.id] = achievement;
        }
    }

    // ID로 업적 찾기
    public AchievementSO GetAchievementById(int id)
    {
        if (achievementById == null)
        {
            Initialize();
        }

        if (achievementById.TryGetValue(id, out AchievementSO achievement))
            return achievement;

        return null;
    }
}
