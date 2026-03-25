using UnityEngine;

[CreateAssetMenu(fileName = "SoundSO", menuName = "DataSO/SoundSO")]
public class SoundSO : ScriptableObject
{
    public int id;
    public AudioClip source;

    public SoundType type;
}
