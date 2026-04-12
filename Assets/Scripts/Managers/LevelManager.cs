using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance;

    [Header("주문 설정")]
    public float orderSpawnInterval = 30f;  // 주문 생성 주기
    public int maxOrderCount = 5;           // 최대 주문 수
    public int minOrderId = 200;            // 최소 요리 id
    public int maxOrderId = 209;            // 최대 요리 id

    [Header("점수 설정")]
    public int penaltyScore = -50;          // 주문 실패 패널티 점수

    [Header("시간 설정")]
    public float stageTimeLimit = 180f;     // 스테이지 제한 시간
    public float ingreTime = 30f;           // 재료 1개당 추가 시간
    public float cookedIngreTime = 60f;     // 1차 조리품 1개당 추가 시간

    [Header("접시 설정")]
    public int maxDishCount = 5;            // 맵에 존재할 수 있는 최대 접시 수

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }
}