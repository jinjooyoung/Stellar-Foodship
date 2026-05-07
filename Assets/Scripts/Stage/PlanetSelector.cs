using UnityEngine;
using TMPro; // 텍스트를 쓰기 위해 꼭 필요합니다!

public class PlanetSelector : MonoBehaviour
{
    [Header("[참조 설정]")]
    public Planet currentMainPlanet;
    public Transform mainSpot;

    [Header("[UI 설정]")]
    public TextMeshProUGUI nameText;       // 행성 이름 UI 연결
    public TextMeshProUGUI difficultyText; // 난이도 UI 연결
    public TextMeshProUGUI infoText;       // 설명 UI 연결

    void Start()
    {
        // 시작하자마자 메인 행성이 있다면 UI를 먼저 한번 보여줍니다.
        if (currentMainPlanet != null)
        {
            UpdatePlanetUI(currentMainPlanet);
        }
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                Planet clickedPlanet = hit.collider.GetComponent<Planet>();
                if (clickedPlanet != null)
                {
                    if (!clickedPlanet.isMain)
                    {
                        SwapPlanets(clickedPlanet);
                    }
                    // 클릭한 행성의 정보를 UI에 표시
                    UpdatePlanetUI(clickedPlanet);
                }
            }
        }
    }

    void SwapPlanets(Planet newMain)
    {
        Vector3 oldMainSubPos = currentMainPlanet.subPosition;
        currentMainPlanet.MoveToSub(oldMainSubPos);

        newMain.MoveToMain(mainSpot.position);
        currentMainPlanet = newMain;
    }

    // UI를 갱신하는 함수
    void UpdatePlanetUI(Planet planet)
    {
        if (nameText != null) nameText.text = planet.info.planetName;
        if (infoText != null) infoText.text = planet.info.description;

        if (difficultyText != null)
        {
            string stars = "난이도: ";
            for (int i = 0; i < 5; i++)
            {
                stars += (i < planet.info.difficulty) ? "★" : "☆";
            }
            difficultyText.text = stars;
        }
    }
}