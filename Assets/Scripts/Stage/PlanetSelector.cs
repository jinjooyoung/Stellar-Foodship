using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement; // <-- 추가: 씬 전환 기능을 위해 필요합니다.

public class PlanetSelector : MonoBehaviour
{
    [Header("[참조 설정]")]
    public Planet currentMainPlanet;
    public Transform mainSpot;

    [Header("[UI 설정]")]
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI difficultyText;
    public TextMeshProUGUI infoText;

    void Awake()
    {
        Time.timeScale = 1f;
    }

    void Start()
    {
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

    // --- 추가된 부분: 버튼을 누르면 호출할 함수 ---
    public void StartStage()
    {
        if (currentMainPlanet != null)
        {
            // 숫자를 "001" 형식의 3자리 문자열로 변환합니다.
            string stageNumberString = currentMainPlanet.info.stageNumber.ToString("D3");
            string sceneName = "Stage_" + stageNumberString;

            Debug.Log(sceneName + "으로 진입합니다!");
            SceneManager.LoadScene(sceneName);
        }
    }
}