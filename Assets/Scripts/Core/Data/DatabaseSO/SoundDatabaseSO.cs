using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SoundDatabaseSO", menuName = "SO/DatabaseSO/SoundDataBaseSO")]
public class SoundDatabaseSO : ScriptableObject
{
    public List<SoundSO> sounds = new List<SoundSO>();

    // 캐싱을 위한 딕셔너리
    private Dictionary<int, SoundSO> soundById;     // ID로 사운드SO 찾기

    public void Initialize()
    {
        soundById = new Dictionary<int, SoundSO>();

        foreach (var sound in sounds)
        {
            soundById[sound.id] = sound;
        }
    }

    // ID로 사운드SO 찾기
    public SoundSO GetSoundById(int id)
    {
        if (soundById == null)
        {
            Initialize();
        }

        if (soundById.TryGetValue(id, out SoundSO sound))
            return sound;

        return null;
    }
}
