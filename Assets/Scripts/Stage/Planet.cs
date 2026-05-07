using UnityEngine;

// 행성 정보를 담는 클래스 (스크립트 상단에 배치)
[System.Serializable]
public class PlanetInfo
{
    public string planetName;    // 행성 이름
    [Range(1, 5)]
    public int difficulty;       // 난이도 (1~5)
    [TextArea]
    public string description;   // 행성 정보/설명
}

public class Planet : MonoBehaviour
{
    public bool isMain = false;
    public bool isSpecialPlanetA = false;
    public PlanetInfo info; // <-- 여기에 정보 입력

    private Vector3 targetPosition;
    private Vector3 targetScale;
    public float lerpSpeed = 5f;

    public Vector3 subPosition;
    public Vector3 mainPosition;

    private readonly Vector3 SCALE_0_5 = new Vector3(0.5f, 0.5f, 0.5f);
    private readonly Vector3 SCALE_1 = new Vector3(1f, 1f, 1f);
    private readonly Vector3 SCALE_2 = new Vector3(2f, 2f, 2f);

    void Awake()
    {
        // 초기 위치/크기 설정
        if (isMain)
        {
            targetPosition = mainPosition;
            targetScale = isSpecialPlanetA ? SCALE_1 : SCALE_2;
        }
        else
        {
            targetPosition = subPosition;
            targetScale = isSpecialPlanetA ? SCALE_0_5 : SCALE_1;
        }
        transform.localPosition = targetPosition;
        transform.localScale = targetScale;
    }

    void Update()
    {
        transform.localPosition = Vector3.Lerp(transform.localPosition, targetPosition, Time.deltaTime * lerpSpeed);
        transform.localScale = Vector3.Lerp(transform.localScale, targetScale, Time.deltaTime * lerpSpeed);
    }

    public void MoveToMain(Vector3 mainPos)
    {
        isMain = true;
        targetPosition = mainPos;
        targetScale = isSpecialPlanetA ? SCALE_1 : SCALE_2;
    }

    public void MoveToSub(Vector3 subPos)
    {
        isMain = false;
        targetPosition = subPos;
        targetScale = isSpecialPlanetA ? SCALE_0_5 : SCALE_1;
    }
}