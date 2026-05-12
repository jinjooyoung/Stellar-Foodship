using UnityEngine;

[System.Serializable]
public class PlanetInfo
{
    public string planetName;
    [Range(1, 5)]
    public int difficulty;
    [TextArea]
    public string description;
    public int stageNumber; // <-- 추가: 스테이지 번호 (예: 1, 2, 3...)
}

public class Planet : MonoBehaviour
{
    public bool isMain = false;
    public bool isSpecialPlanetA = false;
    public PlanetInfo info;

    public Vector3 targetPosition;
    private Vector3 targetScale;
    public float lerpSpeed = 5f;

    public Vector3 subPosition;
    public Vector3 mainPosition;

    private readonly Vector3 SCALE_0_5 = new Vector3(0.5f, 0.5f, 0.5f);
    private readonly Vector3 SCALE_1 = new Vector3(1f, 1f, 1f);
    private readonly Vector3 SCALE_2 = new Vector3(2f, 2f, 2f);

    void Awake()
    {
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