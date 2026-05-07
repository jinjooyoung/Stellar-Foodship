using UnityEngine;
using System.Collections;

public class Planet : MonoBehaviour
{
    public bool isMain = false; // 현재 메인 행성인지 확인

    [Header("[크기 설정]")]
    [Tooltip("이 행성이 특수 행성(A)인가요? 체크 해제 시 일반 행성(B)으로 취급됩니다.")]
    public bool isSpecialPlanetA = false;

    // 타겟 값
    private Vector3 targetPosition;
    private Vector3 targetScale;
    private float lerpSpeed = 5f; // 이동 속도

    // 초기 설정 저장용
    public Vector3 subPosition; // 노란색 박스 위치
    public Vector3 mainPosition; // 빨간색 박스 위치

    // 크기 상수값 정의 (Vector3로 캐싱하여 사용)
    private readonly Vector3 SCALE_0_5 = new Vector3(0.5f, 0.5f, 0.5f);
    private readonly Vector3 SCALE_1 = new Vector3(1f, 1f, 1f);
    private readonly Vector3 SCALE_2 = new Vector3(2f, 2f, 2f);

    void Awake()
    {
        // 타겟값 초기화 (Start에서 해도 되지만 안전하게 Awake에서 한번 더)
        if (isMain)
        {
            targetPosition = mainPosition;
            // 시작하자마자 메인인 행성의 크기 결정
            targetScale = isSpecialPlanetA ? SCALE_1 : SCALE_2;
        }
        else
        {
            targetPosition = subPosition;
            // 시작하자마자 서브인 행성의 크기 결정
            targetScale = isSpecialPlanetA ? SCALE_0_5 : SCALE_1;
        }

        // 초기 스케일 즉시 반영
        transform.localScale = targetScale;
    }

    void Start()
    {
        // PlanetSelector에서 초기 메인 행성을 참조하도록 등록 (Selector가 Start에서 처리할 수도 있음)
        // 만약 Selector가 null을 참조한다면, 여기서 스스로를 등록하는 로직이 필요할 수 있습니다.
        // ex: if(isMain) FindObjectOfType<PlanetSelector>().currentMainPlanet = this;
    }

    void Update()
    {
        // 매 프레임 타겟으로 부드럽게 이동 및 크기 조절
        transform.localPosition = Vector3.Lerp(transform.localPosition, targetPosition, Time.deltaTime * lerpSpeed);
        transform.localScale = Vector3.Lerp(transform.localScale, targetScale, Time.deltaTime * lerpSpeed);
    }

    // 메인으로 전환 (빨간 박스로 이동)
    public void MoveToMain(Vector3 mainPos)
    {
        isMain = true;
        targetPosition = mainPos;

        // [크기 조건 변경]
        // 특수 행성A면 1, 아니면 2
        targetScale = isSpecialPlanetA ? SCALE_1 : SCALE_2;
    }

    // 서브로 전환 (노란 박스로 이동)
    public void MoveToSub(Vector3 subPos)
    {
        isMain = false;
        targetPosition = subPos;

        // [크기 조건 변경]
        // 특수 행성A면 0.5, 아니면 1
        targetScale = isSpecialPlanetA ? SCALE_0_5 : SCALE_1;
    }
}