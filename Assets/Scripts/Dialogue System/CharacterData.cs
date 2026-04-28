using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "New Character", menuName = "Dialogue System/Character Data")]
public class CharacterData : ScriptableObject
{
    [Header("캐릭터 기본 정보")]
    public string characterName; 
    public Color nameColor = Color.white;
    public Sprite characterPortrait;  // 캐릭터 초상화 (선택사항)

    [Header("음성 설정")]
    public AudioClip aSound;  // ㅏ, ㅐ 소리
    public AudioClip eSound;  // ㅓ, ㅔ 소리
    public AudioClip iSound;  // ㅣ 소리
    public AudioClip oSound;  // ㅗ, ㅛ 소리
    public AudioClip uSound;  // ㅜ, ㅠ, ㅡ 소리
    public AudioClip defaultSound;  // 기본 소리

    [Header("음성 특성")]
    [Range(0.5f, 2.0f)]
    public float pitch = 1.0f;  // 음성 높낮이
    [Range(0.3f, 1.0f)]
    public float volume = 1.0f; // 음성 크기

    [Header("타이핑 속도")]
    [Range(0.05f, 0.5f)]
    public float typingSpeed = 0.15f;  // 캐릭터별 말하는 속도

    [Header("캐릭터 설명")]
    [TextArea(2, 4)]
    public string description;  // 캐릭터 설명 (개발용)
}